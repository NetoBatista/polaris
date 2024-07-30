using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Constant;
using Polaris.Domain.Entity;
using Polaris.Domain.Model;
using Polaris.Repository;
using Polaris.Test.Util;

namespace Polaris.Test.Repository
{
    [TestClass]
    public class AuthenticationRepositoryTest
    {
        private AuthenticationRepository _repository;
        private PolarisContext _context;

        [TestInitialize]
        public void Setup()
        {
            _context = DatabaseUtil.Create();
            _repository = new AuthenticationRepository(_context);
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Dispose();
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var entity = new Authentication
            {
                MemberId = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
            };
            entity = await _repository.Create(entity);
            var exists = await _context.Authentication.AnyAsync(x => x.Id == entity!.Id);
            Assert.IsTrue(exists);
        }

        [TestMethod("Should be able authenticate with password")]
        public async Task AuthenticatePassword()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailPassword,
                Password = CryptographyUtil.ConvertToMD5("123456"),
                MemberNavigation = new Member
                {
                    UserId = Guid.NewGuid(),
                    UserNavigation = new User
                    {
                        Name = Guid.NewGuid().ToString(),
                        Email = $"{Guid.NewGuid()}@email.com",
                        Language = UserLanguageConstant.EN_US
                    },
                    ApplicationNavigation = new Application
                    {
                        Name = Guid.NewGuid().ToString()
                    }
                }
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();

            var requestModel = new AuthenticationPasswordModel
            {
                Email = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = entity.MemberNavigation.ApplicationId,
                Password = entity.Password
            };
            var authenticated = await _repository.AuthenticatePassword(requestModel);
            Assert.IsTrue(authenticated);
        }

        [TestMethod("Should not be able authenticate with password")]
        public async Task NoAuthenticatePassword()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailPassword,
                Password = CryptographyUtil.ConvertToMD5("123456"),
                MemberNavigation = new Member
                {
                    UserId = Guid.NewGuid(),
                    UserNavigation = new User
                    {
                        Name = Guid.NewGuid().ToString(),
                        Email = $"{Guid.NewGuid()}@email.com",
                        Language = UserLanguageConstant.EN_US
                    },
                    ApplicationNavigation = new Application
                    {
                        Name = Guid.NewGuid().ToString()
                    }
                }
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();

            var requestModel = new AuthenticationPasswordModel
            {
                Email = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = entity.MemberNavigation.ApplicationId,
                Password = CryptographyUtil.ConvertToMD5("654321"),
            };
            var authenticated = await _repository.AuthenticatePassword(requestModel);
            Assert.IsFalse(authenticated);
        }

        [TestMethod("Should be able authenticate code")]
        public async Task AuthenticateCode()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                Code = "123456",
                CodeExpiration = DateTime.UtcNow.AddMinutes(5),
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();

            var authenticated = await _repository.AuthenticateCode(entity);
            Assert.IsTrue(authenticated);
        }

        [TestMethod("Should not be able authenticate code")]
        public async Task NotAuthenticateCode()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                Code = "123456",
                CodeExpiration = DateTime.UtcNow.AddMinutes(5),
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            entity.Code = "654321";
            var authenticated = await _repository.AuthenticateCode(entity);
            Assert.IsFalse(authenticated);
        }

        [TestMethod("Should be able change password")]
        public async Task ChangePassword()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailPassword,
                Password = CryptographyUtil.ConvertToMD5("123456"),
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            entity.Password = CryptographyUtil.ConvertToMD5("654321");
            var response = await _repository.ChangePassword(entity);
            Assert.IsTrue(response);
        }

        [TestMethod("Should not be able change password")]
        public async Task NotChangePassword()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            entity.Password = CryptographyUtil.ConvertToMD5("123456");
            var response = await _repository.ChangePassword(entity);
            Assert.IsFalse(response);
        }

        [TestMethod("Should be able get by email and application")]
        public async Task GetByEmailApplication()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailPassword,
                Password = CryptographyUtil.ConvertToMD5("123456"),
                MemberNavigation = new Member
                {
                    UserId = Guid.NewGuid(),
                    UserNavigation = new User
                    {
                        Name = Guid.NewGuid().ToString(),
                        Email = $"{Guid.NewGuid()}@email.com",
                        Language = UserLanguageConstant.EN_US
                    },
                    ApplicationNavigation = new Application
                    {
                        Name = Guid.NewGuid().ToString()
                    }
                }
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();

            var requestModel = new AuthenticationByUserApplicationModel
            {
                Email = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = entity.MemberNavigation.ApplicationId
            };
            var response = await _repository.GetByEmailApplication(requestModel);
            Assert.IsNotNull(response);
        }

        [TestMethod("Should not be able get by email and application")]
        public async Task NotGetByEmailApplication()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailPassword,
                Password = CryptographyUtil.ConvertToMD5("123456"),
                MemberNavigation = new Member
                {
                    UserId = Guid.NewGuid(),
                    UserNavigation = new User
                    {
                        Name = Guid.NewGuid().ToString(),
                        Email = $"{Guid.NewGuid()}@email.com",
                        Language = UserLanguageConstant.EN_US
                    },
                    ApplicationNavigation = new Application
                    {
                        Name = Guid.NewGuid().ToString()
                    }
                }
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();

            var requestModel = new AuthenticationByUserApplicationModel
            {
                Email = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = Guid.NewGuid(),
            };
            var response = await _repository.GetByEmailApplication(requestModel);
            Assert.IsNull(response);
        }

        [TestMethod("Should be able get by refresh token")]
        public async Task GetByRefreshToken()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                RefreshToken = Guid.NewGuid().ToString(),
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();

            var response = await _repository.GetByRefreshToken(entity);
            Assert.IsNotNull(response);
        }

        [TestMethod("Should not be able get by refresh token")]
        public async Task NotGetByRefreshToken()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                RefreshToken = Guid.NewGuid().ToString(),
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            entity.RefreshToken = Guid.NewGuid().ToString();
            var response = await _repository.GetByRefreshToken(entity);
            Assert.IsNull(response);
        }

        [TestMethod("Should be able refresh token")]
        public async Task RefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                RefreshToken = refreshToken,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            var response = await _repository.RefreshToken(entity);
            Assert.AreNotEqual(response.RefreshToken, refreshToken);
        }

        [TestMethod("Should be able change type to email only")]
        public async Task ChangeTypeToEmailOnly()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailPassword,
                Password = CryptographyUtil.ConvertToMD5("123456"),
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            entity.Type = AuthenticationTypeConstant.EmailOnly;
            var response = await _repository.ChangeType(entity);
            Assert.IsNull(response.Password);
        }

        [TestMethod("Should be able change type to email password")]
        public async Task ChangeTypeToEmailPassword()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            entity.Type = AuthenticationTypeConstant.EmailPassword;
            entity.Password = CryptographyUtil.ConvertToMD5("123456");
            var response = await _repository.ChangeType(entity);
            Assert.IsNotNull(response.Password);
        }

        [TestMethod("Should be able generate code")]
        public async Task GenerateCode()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            var response = await _repository.GenerateCode(entity);
            var fieldsNotNull = !string.IsNullOrEmpty(response.Code) &&
                                response.CodeAttempt != null &&
                                response.CodeExpiration != null;
            Assert.IsTrue(fieldsNotNull);
        }

        [TestMethod("Should be able clear code confirmarion")]
        public async Task ClearCodeConfirmation()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                Code = "123456",
                CodeExpiration = DateTime.UtcNow.AddMinutes(5),
                CodeAttempt = 1,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            await _repository.ClearCodeConfirmation(entity);
            var authentication = _context.Authentication.First();
            var fieldsNull = string.IsNullOrEmpty(authentication.Code) &&
                             authentication.CodeAttempt == null &&
                             authentication.CodeExpiration == null;
            Assert.IsTrue(fieldsNull);
        }

        [TestMethod("Should be able validate code")]
        public async Task ValidateCode()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                Code = "123456",
                CodeExpiration = DateTime.UtcNow.AddMinutes(5),
                CodeAttempt = 1,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            var response = await _repository.CanValidateCode(entity);
            Assert.IsTrue(response);
        }

        [TestMethod("Should not be able validate code")]
        public async Task NotValidateCode()
        {
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Type = AuthenticationTypeConstant.EmailOnly,
                Code = "123456",
                CodeExpiration = DateTime.UtcNow.AddMinutes(-10),
                CodeAttempt = 1,
            };
            _context.Authentication.Add(entity);
            await _context.SaveChangesAsync();
            var response = await _repository.CanValidateCode(entity);
            Assert.IsFalse(response);
        }
    }
}
