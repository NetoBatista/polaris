﻿using Polaris.Domain.Entity;

namespace Polaris.Domain.Interface.Repository
{
    public interface IApplicationRepository
    {
        Task<Application> Create(Application application);

        Task<Application> Update(Application application);

        Task Remove(Application application);

        Task<bool> AlreadyCreated(Application application);

        Task<bool> Exists(Application application);

        Task<bool> AnyMember(Application application);

        Task<List<Application>> GetAll();
    }
}