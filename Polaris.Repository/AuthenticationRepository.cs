﻿using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Model.Authentication;

namespace Polaris.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly PolarisContext _context;
        public AuthenticationRepository(PolarisContext context)
        {
            _context = context;
        }

        public async Task<Authentication> Create(Authentication authentication)
        {
            await _context.AddAsync(authentication);
            await _context.SaveChangesAsync();
            return authentication;
        }

        public async Task<bool> AuthenticatePassword(AuthenticationPasswordModel model)
        {
            var email = model.Email ?? string.Empty;
            return await _context.Authentication.AsNoTracking()
                                                .Include(x => x.MemberNavigation)
                                                .ThenInclude(x => x.UserNavigation)
                                                .AnyAsync(x => x.MemberNavigation.UserNavigation.Email.ToUpper() == email.ToUpper() &&
                                                                           x.MemberNavigation.ApplicationId == model.ApplicationId &&
                                                                           x.Password == model.Password);
        }

        public async Task<bool> AuthenticateCode(Authentication authentication)
        {
            var entity = await _context.Authentication.AsNoTracking()
                                                      .FirstAsync(x => x.Id == authentication.Id);
            entity.CodeAttempt = (entity.CodeAttempt ?? 0) + 1;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity.Code == authentication.Code;
        }

        public async Task<bool> ChangePassword(Authentication authentication)
        {
            var entity = await _context.Authentication.FirstOrDefaultAsync(x => x.Id == authentication.Id);
            if (entity == null)
            {
                return false;
            }
            entity.Password = authentication.Password;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<Authentication?> GetById(Guid id)
        {
            return _context.Authentication.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Authentication?> GetByEmailApplication(AuthenticationByUserApplicationModel model)
        {
            var email = model.Email ?? string.Empty;
            return _context.Authentication.AsNoTracking()
                                          .Include(x => x.MemberNavigation)
                                          .ThenInclude(x => x.UserNavigation)
                                          .Include(x => x.MemberNavigation)
                                          .ThenInclude(x => x.ApplicationNavigation)
                                          .FirstOrDefaultAsync(x => x.MemberNavigation.UserNavigation.Email.ToUpper() == email.ToUpper() &&
                                                                    x.MemberNavigation.ApplicationNavigation.Id == model.ApplicationId);
        }

        public async Task<Authentication> GenerateCode(Authentication authentication)
        {
            var entity = await _context.Authentication.AsNoTracking()
                                                      .FirstAsync(x => x.Id == authentication.Id);
            entity.Code = CodeConfirmation();
            entity.CodeExpiration = DateTime.UtcNow.AddMinutes(5);
            entity.CodeAttempt = 0;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task ClearCodeConfirmation(Authentication authentication)
        {
            var entity = await _context.Authentication.FirstAsync(x => x.Id == authentication.Id);
            entity.Code = null;
            entity.CodeExpiration = null;
            entity.CodeAttempt = null;
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanValidateCode(Authentication authentication)
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Authentication.AsNoTracking()
                                                .AnyAsync(x => x.Id == authentication.Id &&
                                                               x.CodeAttempt < 5 &&
                                                               x.CodeExpiration > currentDate);
        }

        private string CodeConfirmation()
        {
            var response = string.Empty;
            var random = new Random();
            while (response.Length != 6)
            {
                response += random.Next(1, 9);
            }
            return response;
        }
    }
}
