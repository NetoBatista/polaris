using Moq;
using Polaris.Domain.Configuration;
using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Model;
using Polaris.Domain.Validator.Authentication;
using Polaris.Service;
using System.Net;

namespace Polaris.Test.Service
{
    [TestClass]
    public class AuthenticationServiceTest
    {
        public AuthenticationServiceTest()
        {
            TokenConfig.Expire = 5;
            TokenConfig.Secret = Guid.NewGuid().ToString();
        }

        private Mock<IAuthenticationRepository> _repository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IMemberRepository> _memberRepository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IAuthenticationRepository>();
            _memberRepository = new Mock<IMemberRepository>();
            _userRepository = new Mock<IUserRepository>();
        }

        [TestCleanup]
        public void Teardown()
        {
            _repository.Reset();
        }

        private AuthenticationService CreateService()
        {
            var authenticatorValidator = new AuthenticationValidator();
            var authenticatorFirebaseValidator = new AuthenticationFirebaseValidator();
            var authenticatorRefreshTokenValidator = new AuthenticationRefreshTokenValidator();
            var authenticatorGenerateCodeValidator = new AuthenticationGenerateCodeValidator();
            var authenticationChangePasswordValidator = new AuthenticationChangePasswordValidator(_repository.Object);
            return new AuthenticationService(_repository.Object,
                                            _userRepository.Object,
                                            _memberRepository.Object,
                                            authenticatorValidator,
                                            authenticatorFirebaseValidator,
                                            authenticatorGenerateCodeValidator,
                                            authenticatorRefreshTokenValidator,
                                            authenticationChangePasswordValidator);
        }

        [TestMethod("Should be able authenticate EmailOnly")]
        public async Task AuthenticateEmailOnly()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able authenticate EmailPassword")]
        public async Task AuthenticateEmailPassword()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = $"{Guid.NewGuid()}@email.com",
                Password = Guid.NewGuid().ToString()
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.AuthenticatePassword(It.IsAny<AuthenticationPasswordModel>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able authenticate EmailPassword InvalidCredentials")]
        public async Task NotAuthenticateEmailPasswordInvalidCredentials()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = $"{Guid.NewGuid()}@email.com",
                Password = Guid.NewGuid().ToString()
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.AuthenticatePassword(It.IsAny<AuthenticationPasswordModel>()))
                       .ReturnsAsync(false);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able authenticate EmailOnly InvalidCredentials")]
        public async Task NotAuthenticateEmailOnlyInvalidCredentials()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(false);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able authenticate EmailOnly ErrorValidateCode")]
        public async Task NotAuthenticateEmailOnlyErrorValidateCode()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(false);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able authenticate Email not found")]
        public async Task NotAuthenticateEmailNotFound()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()));
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }


        [TestMethod("Should not be able authenticate NoCodePassword")]
        public async Task NotAuthenticateNoCodePassword()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able authenticate EmailEmpty")]
        public async Task NotAuthenticateEmailEmpty()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = string.Empty,
                Code = "123456"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able authenticate Incorrect email")]
        public async Task NotAuthenticateIncorrectEmail()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = "abc",
                Code = "123456"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able authenticate Incorrect Application")]
        public async Task NotAuthenticateIncorrectApplication()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.Empty,
                Email = $"{Guid.NewGuid()}@email.com",
                Code = "123456"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };
            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.CanValidateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _userRepository.Setup(x => x.Get(It.IsAny<User>()))
                       .ReturnsAsync(entity.MemberNavigation.UserNavigation);

            var service = CreateService();

            var response = await service.Authenticate(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able GenerateCode")]
        public async Task GenerateCode()
        {
            var request = new AuthenticationGenerateCodeRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.GenerateCode(It.IsAny<Authentication>()));


            var service = CreateService();
            var response = await service.GenerateCode(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able GenerateCode NotFound")]
        public async Task NotGenerateCodeNotFound()
        {
            var request = new AuthenticationGenerateCodeRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = $"{Guid.NewGuid()}@email.com"
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()));
            _repository.Setup(x => x.GenerateCode(It.IsAny<Authentication>()));


            var service = CreateService();
            var response = await service.GenerateCode(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able GenerateCode IncorrectApplication")]
        public async Task NotGenerateCodeIncorrectApplication()
        {
            var request = new AuthenticationGenerateCodeRequestDTO
            {
                ApplicationId = Guid.Empty,
                Email = $"{Guid.NewGuid()}@email.com"
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()));
            _repository.Setup(x => x.GenerateCode(It.IsAny<Authentication>()));


            var service = CreateService();
            var response = await service.GenerateCode(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able GenerateCode IncorrectEmail")]
        public async Task NotGenerateCodeIncorrectEmail()
        {
            var request = new AuthenticationGenerateCodeRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = Guid.NewGuid().ToString()
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()));
            _repository.Setup(x => x.GenerateCode(It.IsAny<Authentication>()));


            var service = CreateService();
            var response = await service.GenerateCode(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able GenerateCode EmptyEmail")]
        public async Task NotGenerateCodeEmptyEmail()
        {
            var request = new AuthenticationGenerateCodeRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = string.Empty
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()));
            _repository.Setup(x => x.GenerateCode(It.IsAny<Authentication>()));


            var service = CreateService();
            var response = await service.GenerateCode(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able RefreshToken")]
        public async Task RefreshToken()
        {
            var request = new AuthenticationRefreshTokenRequestDTO
            {
                RefreshToken = Guid.NewGuid()
            };

            var memberId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var userId = Guid.NewGuid(); var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = applicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = applicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = $"{Guid.NewGuid()}@email.com",
                    }
                }
            };

            _repository.Setup(x => x.GetByRefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ClearCodeConfirmation(It.IsAny<Authentication>()));
            _memberRepository.Setup(x => x.Get(It.IsAny<Member>()))
                             .ReturnsAsync([entity.MemberNavigation]);

            var service = CreateService();
            var response = await service.RefreshToken(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able RefreshToken InvalidCredentials")]
        public async Task NotRefreshTokenInvalidCredentials()
        {
            var request = new AuthenticationRefreshTokenRequestDTO
            {
                RefreshToken = Guid.NewGuid()
            };

            var memberId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var userId = Guid.NewGuid(); var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = applicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = applicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = $"{Guid.NewGuid()}@email.com",
                    }
                }
            };

            _repository.Setup(x => x.GetByRefreshToken(It.IsAny<Authentication>()));
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ClearCodeConfirmation(It.IsAny<Authentication>()));
            _memberRepository.Setup(x => x.Get(It.IsAny<Member>()))
                             .ReturnsAsync([entity.MemberNavigation]);

            var service = CreateService();
            var response = await service.RefreshToken(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able RefreshToken IncorrectToken")]
        public async Task NotRefreshTokenIncorrectToken()
        {
            var request = new AuthenticationRefreshTokenRequestDTO
            {
                RefreshToken = Guid.Empty
            };

            _repository.Setup(x => x.GetByRefreshToken(It.IsAny<Authentication>()));
            _repository.Setup(x => x.RefreshToken(It.IsAny<Authentication>()));
            _repository.Setup(x => x.ClearCodeConfirmation(It.IsAny<Authentication>()));
            _memberRepository.Setup(x => x.Get(It.IsAny<Member>()));

            var service = CreateService();
            var response = await service.RefreshToken(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able Change Password")]
        public async Task ChangePassword()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Password = CryptographyUtil.ConvertToMD5("123456"),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.AuthenticateCode(It.IsAny<Authentication>()))
                       .ReturnsAsync(true);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able Change Password CurrentPasswordLessThen6")]
        public async Task NotChangePasswordCurrentPasswordLessThen6()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Password = CryptographyUtil.ConvertToMD5("123456"),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able Change Password LessThen6")]
        public async Task NotChangePasswordLessThen6()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Password = CryptographyUtil.ConvertToMD5("123456"),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able Change Password Dont match")]
        public async Task NotChangePasswordDontMatch()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Password = CryptographyUtil.ConvertToMD5("12345678"),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able Change Password CurrentPasswordEmpty")]
        public async Task NotChangePasswordCurrentPasswordEmpty()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Password = CryptographyUtil.ConvertToMD5("123456"),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able Change Password Empty")]
        public async Task NotChangePasswordEmpty()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = string.Empty,
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                Password = CryptographyUtil.ConvertToMD5("123456"),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able Change Password IncorrectTypeAuthentication")]
        public async Task NotChangePasswordIncorrectTypeAuthentication()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var memberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new Authentication
            {
                Id = Guid.NewGuid(),
                RefreshToken = Guid.NewGuid().ToString(),
                MemberNavigation = new Member
                {
                    Id = memberId,
                    UserId = userId,
                    ApplicationId = request.ApplicationId,
                    ApplicationNavigation = new Application
                    {
                        Id = request.ApplicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Name = Guid.NewGuid().ToString(),
                        Email = request.Email,
                    }
                }
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()))
                       .ReturnsAsync(entity);
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able Change Password UserNotFound")]
        public async Task NotChangePasswordUserNotFound()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            _repository.Setup(x => x.GetByEmailApplication(It.IsAny<AuthenticationByUserApplicationModel>()));
            _repository.Setup(x => x.ChangePassword(It.IsAny<Authentication>()));

            var service = CreateService();
            var response = await service.ChangePassword(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }
    }
}
