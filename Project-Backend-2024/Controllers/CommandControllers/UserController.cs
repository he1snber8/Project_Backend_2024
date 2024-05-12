﻿using Microsoft.AspNetCore.Mvc;
using Project_Backend_2024.DTO;
using Project_Backend_2024.Facade.Exceptions;
using Project_Backend_2024.Services.Interfaces.Commands;
using Project_Backend_2024.Facade.Models;
using Project_Backend_2024.Facade.Responses;
using Project_Backend_2024.Services.TokenGenerators;
using Project_Backend_2024.Services.TokenValidators;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Project_Backend_2024.Controllers.CommandControllers;

[ApiController]
[Route("commands/[controller]")]
public class UserController : Controller
{
    private readonly IUserCommandService _userCommandService;
    private readonly AccessTokenGenerator _accessTokenGenerator;
    private readonly RefreshTokenGenerator _refreshTokenGenerator;
    private readonly RefreshTokenValidator _refreshTokenValidator;
    private readonly IRefreshTokenCommandService _refreshTokenRepository;
    private readonly AuthConfiguration _authConfiguration;

    public UserController(IUserCommandService userCommandService, AccessTokenGenerator accessTokenGenerator,
        RefreshTokenGenerator refreshTokenGenerator, RefreshTokenValidator refreshTokenValidator,
        IRefreshTokenCommandService refreshTokenRepository, AuthConfiguration authConfiguration)
    {
        _userCommandService = userCommandService;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenValidator = refreshTokenValidator;
        _refreshTokenRepository = refreshTokenRepository;
        _authConfiguration = authConfiguration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Insert([FromBody] RegisterUserModel model)
    {
        try
        {
            await _userCommandService.Register(model);
            return Ok("User registered successfully!");
        }
        catch (UsernameValidationException ex)
        {
            return BadRequest($"Username validation failed: {ex.Message}");
        }
        catch (PasswordValidationException ex)
        {
            return BadRequest($"Password validation failed: {ex.Message}");
        }
        catch (EmailValidationException ex)
        {
            return BadRequest($"Email validation failed: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] UserLoginModel loginModel)
    {
        (bool isAuthenticated, User user) = _userCommandService.AutheticateLogin(loginModel);

        if (isAuthenticated)
        {
            (string freshToken, string refreshToken) =
            (_accessTokenGenerator.GenerateToken(user), 
            _refreshTokenGenerator.GenerateRefreshToken());

            var token = await _refreshTokenRepository.GetByUserId(user.Id);

            if (token == null)
            {
                await _refreshTokenRepository.Insert(new RefreshTokenModel { UserID = user.Id, Token = refreshToken, 
                    ExpirationDate = DateTime.Now.AddMinutes(_authConfiguration.RefreshTokenExpirationMinutes) });
                return Ok(new AuthenticatedUserResponse(freshToken, refreshToken));
            }

            else if (token.ExpirationDate < DateTime.Now || token.isActive is not true)
            {
                await _refreshTokenRepository.Update(token.Id, new RefreshTokenModel { UserID = user.Id, Token = refreshToken,
                    ExpirationDate = DateTime.Now.AddMinutes(_authConfiguration.RefreshTokenExpirationMinutes), isActive = true });
                return Ok(new AuthenticatedUserResponse(freshToken, refreshToken));

            }

            return Ok(new AuthenticatedUserResponse(freshToken, token.Token));
            
        }

        return Unauthorized();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
    {
        if (!_refreshTokenValidator.Validate(refreshRequest.refreshToken)) return Unauthorized();

        var tokenByToken = await _refreshTokenRepository.GetByToken(refreshRequest.refreshToken);

        if (tokenByToken.isActive is not true) return Unauthorized("Operation failed due to invalid token");

        var user = await _userCommandService.RetrieveUser(tokenByToken.UserID);

        string freshToken = _accessTokenGenerator.GenerateToken(user!);

        return Ok(new AccessToken(freshToken));
    }

    [Authorize]
    [HttpDelete("logout")]
    public async Task<IActionResult> LogOut()
    {
        var userId = User.FindFirstValue("Id");

        int.TryParse(userId, out int tokenId);

        await _refreshTokenRepository.InvalidateTokenByUser(tokenId);

        return Ok("Logged out!");
    }
}



