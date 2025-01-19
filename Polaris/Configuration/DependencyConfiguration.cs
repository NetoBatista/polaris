using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Configuration;
using Polaris.Domain.Dto.Application;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Dto.Member;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Validator.Application;
using Polaris.Domain.Validator.Authentication;
using Polaris.Repository;
using Polaris.Service;

namespace Polaris.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyConfiguration
    {
        public static void InjectDependencies(this IServiceCollection services)
        {
            InjectDataBase(services);
            InjectRepository(services);
            InjectServices(services);
            InjectValidator(services);
        }

        private static void InjectDataBase(IServiceCollection services)
        {
            services.AddDbContext<PolarisContext>(options =>
            {
                options.UseSqlServer(DatabaseConfig.ConnectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Transient);
        }

        private static void InjectRepository(IServiceCollection services)
        {
            services.AddTransient<IMigrationRepository, MigrationRepository>();
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMemberRepository, MemberRepository>();
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        }

        private static void InjectServices(IServiceCollection services)
        {
            services.AddTransient<IMigrationService, MigrationService>();
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMemberService, MemberService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        private static void InjectValidator(IServiceCollection services)
        {
            services.AddTransient<IValidator<ApplicationCreateRequestDTO>, ApplicationCreateValidator>();
            services.AddTransient<IValidator<ApplicationUpdateRequestDTO>, ApplicationUpdateValidator>();
            services.AddTransient<IValidator<ApplicationRemoveRequestDTO>, ApplicationRemoveValidator>();

            services.AddTransient<IValidator<UserCreateRequestDTO>, UserCreateValidator>();
            services.AddTransient<IValidator<UserUpdateRequestDTO>, UserUpdateValidator>();
            services.AddTransient<IValidator<UserRemoveRequestDTO>, UserRemoveValidator>();

            services.AddTransient<IValidator<MemberCreateRequestDTO>, MemberCreateValidator>();
            services.AddTransient<IValidator<MemberRemoveRequestDTO>, MemberRemoveValidator>();

            services.AddTransient<IValidator<AuthenticationRequestDTO>, AuthenticationValidator>();
            services.AddTransient<IValidator<AuthenticationFirebaseRequestDTO>, AuthenticationFirebaseValidator>();
            services.AddTransient<IValidator<AuthenticationRefreshTokenRequestDTO>, AuthenticationRefreshTokenValidator>();
            services.AddTransient<IValidator<AuthenticationGenerateCodeRequestDTO>, AuthenticationGenerateCodeValidator>();
            services.AddTransient<IValidator<AuthenticationChangePasswordRequestDTO>, AuthenticationChangePasswordValidator>();
        }
    }
}
