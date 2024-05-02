﻿using Project_Backend_2024.DTO;

namespace Project_Backend_2024.Facade.Interfaces;

public interface IAccessToken<TIdentityApplicable>
    where TIdentityApplicable : IAuthenticatable, IMailApplicable
{
    public string GenerateToken(TIdentityApplicable identity);
}