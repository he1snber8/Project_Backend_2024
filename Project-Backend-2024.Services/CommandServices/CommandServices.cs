﻿using AutoMapper;
using Project_Backend_2024.DTO;
using Project_Backend_2024.Facade.BasicOperations;
using Project_Backend_2024.Facade.Interfaces;
using Project_Backend_2024.Services.Interfaces.Commands;
using Project_Backend_2024.Services.Models;

namespace Project_Backend_2024.Services.CommandServices;

public class UserCommandService : BaseCommandService<UserModel, User, IUserRepository>, IUserCommandService
{
    public UserCommandService(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository repository) : base(unitOfWork, mapper, repository) { }

    public override async Task<int> Insert(UserModel model)
    {
        if (!model.ValidateUsername()) throw new Exception(model.Username);
        if (!model.ValidatePassword()) throw new Exception(model.Password);
        if (!model.ValidateEmail()) throw new Exception(model.Email);

        return await base.Insert(model);
    }
}

public class ProjectCommandService : BaseCommandService<ProjectModel, Project, IProjectRepository>, IProjectCommandService
{
    public ProjectCommandService(IUnitOfWork unitOfWork, IMapper mapper, IProjectRepository repository) : base(unitOfWork, mapper, repository) { }
}

public class AppliedProjectCommandService : BaseCommandService<AppliedProjectModel, AppliedProject, IAppliedProjectRepository>, IAppliedProjectCommandService
{
    public AppliedProjectCommandService(IUnitOfWork unitOfWork, IMapper mapper, IAppliedProjectRepository repository) : base(unitOfWork, mapper, repository) { }
}

public class SkillCommandService : BaseCommandService<SkillModel, Skill, ISkillRepository>, ISkillCommandService
{
    public SkillCommandService(IUnitOfWork unitOfWork, IMapper mapper, ISkillRepository repository) : base(unitOfWork, mapper, repository) { }
}

public class UserSkillsCommandService : BaseCommandService<UserSkillsModel, UserSkills, IUserSkillsRepository>, IUserSkillsCommandService
{
    public UserSkillsCommandService(IUnitOfWork unitOfWork, IMapper mapper, IUserSkillsRepository repository) : base(unitOfWork, mapper, repository) { }
}