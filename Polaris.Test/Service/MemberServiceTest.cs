using Moq;
using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Member;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Validator.Application;
using Polaris.Service;
using System.Net;

namespace Polaris.Test.Service
{
    [TestClass]
    public class MemberServiceTest
    {
        private Mock<IMemberRepository> _repository;
        private Mock<IAuthenticationRepository> _authenticationRepository;
        private Mock<IApplicationRepository> _applicationRepository;
        private Mock<IUserRepository> _userRepository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IMemberRepository>();
            _applicationRepository = new Mock<IApplicationRepository>();
            _userRepository = new Mock<IUserRepository>();
            _authenticationRepository = new Mock<IAuthenticationRepository>();
        }

        [TestCleanup]
        public void Teardown()
        {
            _repository.Reset();
            _authenticationRepository.Reset();
            _applicationRepository.Reset();
            _userRepository.Reset();
        }

        private MemberService CreateService()
        {
            var createValidator = new MemberCreateValidator(_applicationRepository.Object, _userRepository.Object, _repository.Object);
            var removeValidator = new MemberRemoveValidator(_repository.Object);
            return new MemberService(_repository.Object, _authenticationRepository.Object, createValidator, removeValidator);
        }

        [TestMethod("Should be able create EmailOnly")]
        public async Task CreateEmailOnly()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            _applicationRepository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);
            _userRepository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Create(It.IsAny<Member>())).ReturnsAsync(member);
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(false);
            _authenticationRepository.Setup(x => x.Create(It.IsAny<Authentication>()));

            var service = CreateService();
            var request = new MemberCreateRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able create EmailPassword")]
        public async Task CreateEmailPassword()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            _applicationRepository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);
            _userRepository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Create(It.IsAny<Member>())).ReturnsAsync(member);
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(false);
            _authenticationRepository.Setup(x => x.Create(It.IsAny<Authentication>()));

            var service = CreateService();
            var request = new MemberCreateRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Password = "123456",
                UserId = Guid.NewGuid()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able create PasswordLessThen6")]
        public async Task NotCreatePasswordLessThen6()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            _applicationRepository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);
            _userRepository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Create(It.IsAny<Member>())).ReturnsAsync(member);
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(false);
            _authenticationRepository.Setup(x => x.Create(It.IsAny<Authentication>()));

            var service = CreateService();
            var request = new MemberCreateRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Password = "123",
                UserId = Guid.NewGuid()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create email not found")]
        public async Task NotCreateEmailNotFound()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            _applicationRepository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);
            _userRepository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);
            _repository.Setup(x => x.Create(It.IsAny<Member>())).ReturnsAsync(member);
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(false);
            _authenticationRepository.Setup(x => x.Create(It.IsAny<Authentication>()));

            var service = CreateService();
            var request = new MemberCreateRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create application not found")]
        public async Task NotCreateApplicationNotFound()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            _applicationRepository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);
            _userRepository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Create(It.IsAny<Member>())).ReturnsAsync(member);
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(false);
            _authenticationRepository.Setup(x => x.Create(It.IsAny<Authentication>()));

            var service = CreateService();
            var request = new MemberCreateRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create member exists")]
        public async Task NotCreateMemberExists()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            _applicationRepository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);
            _userRepository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Create(It.IsAny<Member>())).ReturnsAsync(member);
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(true);
            _authenticationRepository.Setup(x => x.Create(It.IsAny<Authentication>()));

            var service = CreateService();
            var request = new MemberCreateRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(true);
            _repository.Setup(x => x.Remove(It.IsAny<Member>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new MemberRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able remove")]
        public async Task NotRemove()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Member>())).ReturnsAsync(false);
            _repository.Setup(x => x.Remove(It.IsAny<Member>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new MemberRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able get by user")]
        public async Task GetByUser()
        {
            var applicationId = Guid.NewGuid();
            var authenticationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var members = new List<Member>
            {
                new Member
                {
                    ApplicationId = applicationId,
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ApplicationNavigation = new Application
                    {
                        Id= applicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    AuthenticationNavigation = new Authentication
                    {
                        Id = authenticationId,
                        RefreshTokenNavigation = new List<RefreshToken>
                        {
                            new RefreshToken()
                        }
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Email = $"{Guid.NewGuid()}@email.com",
                        Language = UserLanguageConstant.EN_US,
                        Name = Guid.NewGuid().ToString()
                    }
                }
            };
            _repository.Setup(x => x.Get(It.IsAny<Member>())).ReturnsAsync(members);
            _repository.Setup(x => x.Remove(It.IsAny<Member>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new MemberGetUserRequestDTO
            {
                UserId = userId
            };
            var response = await service.GetByUser(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able get by Application")]
        public async Task GetByApplication()
        {
            var applicationId = Guid.NewGuid();
            var authenticationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var members = new List<Member>
            {
                new Member
                {
                    ApplicationId = applicationId,
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ApplicationNavigation = new Application
                    {
                        Id= applicationId,
                        Name = Guid.NewGuid().ToString(),
                    },
                    AuthenticationNavigation = new Authentication
                    {
                        Id = authenticationId,
                        RefreshTokenNavigation = new List<RefreshToken>
                        {
                            new RefreshToken()
                        }
                    },
                    UserNavigation = new User
                    {
                        Id = userId,
                        Email = $"{Guid.NewGuid()}@email.com",
                        Language = UserLanguageConstant.EN_US,
                        Name = Guid.NewGuid().ToString()
                    }
                }
            };
            _repository.Setup(x => x.Get(It.IsAny<Member>())).ReturnsAsync(members);
            _repository.Setup(x => x.Remove(It.IsAny<Member>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new MemberGetApplicationRequestDTO
            {
                ApplicationId = applicationId
            };
            var response = await service.GetByApplication(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}
