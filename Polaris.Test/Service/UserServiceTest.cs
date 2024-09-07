using Moq;
using Polaris.Domain.Constant;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Validator.Application;
using Polaris.Service;
using System.Net;

namespace Polaris.Test.Service
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<IUserRepository> _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IUserRepository>();
        }

        [TestCleanup]
        public void Teardown()
        {
            _repository.Reset();
        }

        private UserService CreateService()
        {
            var createValidator = new UserCreateValidator(_repository.Object);
            var updateValidator = new UserUpdateValidator(_repository.Object);
            var removeValidator = new UserRemoveValidator(_repository.Object);
            return new UserService(_repository.Object, createValidator, updateValidator, removeValidator);
        }

        [TestMethod("Should be able Create")]
        public async Task Create()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = entity.Email,
                Language = entity.Language,
                Name = entity.Name,
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able create - AlreadyCreatedValidation")]
        public async Task NotCreateAlreadyCreatedValidation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = entity.Email,
                Language = entity.Language,
                Name = entity.Name,
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - NameNullValidation")]
        public async Task NotCreateNameNullValidation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = entity.Email,
                Language = entity.Language,
                Name = string.Empty,
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - NameLess3Validation")]
        public async Task NotCreateNameLess3Validation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = entity.Email,
                Language = entity.Language,
                Name = "ab",
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - MoreThen256Validation")]
        public async Task NotCreateNameMoreThen256Validation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var name = string.Empty;
            while (name.Length <= 256)
            {
                name += Guid.NewGuid().ToString();
            }
            var request = new UserCreateRequestDTO
            {
                Email = entity.Email,
                Language = entity.Language,
                Name = name,
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - EmailEmpty")]
        public async Task NotCreateEmailEmptyValidation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = string.Empty,
                Language = entity.Language,
                Name = Guid.NewGuid().ToString(),
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - IncorrectEmail")]
        public async Task NotCreateIncorrectEmailValidation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = Guid.NewGuid().ToString(),
                Language = entity.Language,
                Name = Guid.NewGuid().ToString(),
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - IncorrectLanguage")]
        public async Task NotCreateIncorrectLanguageValidation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Language = "en",
                Name = Guid.NewGuid().ToString(),
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create - LanguageEmpty")]
        public async Task NotCreateLanguageEmptyValidation()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(entity);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserCreateRequestDTO
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Language = string.Empty,
                Name = Guid.NewGuid().ToString(),
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able get by name filter")]
        public async Task GetByNameFilter()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Get(It.IsAny<User>())).ReturnsAsync(entity);

            var service = CreateService();
            var request = new UserGetRequestDTO
            {
                Email = entity.Email,
            };
            var response = await service.Get(request);
            var users = (List<UserResponseDTO>?)response.Value;
            Assert.AreEqual(users!.Count, 1);
        }

        [TestMethod("Should be able get by id filter")]
        public async Task GetByIdFilter()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Get(It.IsAny<User>())).ReturnsAsync(entity);

            var service = CreateService();
            var request = new UserGetRequestDTO
            {
                Id = entity.Id
            };
            var response = await service.Get(request);
            var users = (List<UserResponseDTO>?)response.Value;
            Assert.AreEqual(users!.Count, 1);
        }

        [TestMethod("Should be able get by application filter")]
        public async Task GetByApplicationFilter()
        {
            var userId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var entity = new User
            {
                Id = userId,
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US,
                MemberNavigation = new List<Member>
                {
                    new Member
                    {
                        Id = Guid.NewGuid(),
                        ApplicationId = applicationId,
                        UserId = userId,
                        ApplicationNavigation = new Application
                        {
                            Id = applicationId,
                            Name = Guid.NewGuid().ToString()
                        }
                    }
                }
            };
            _repository.Setup(x => x.GetByApplication(It.IsAny<Guid>()))
                       .ReturnsAsync(new List<User> { entity });

            var service = CreateService();
            var request = new UserGetRequestDTO
            {
                ApplicationId = entity.MemberNavigation.First().ApplicationId
            };
            var response = await service.Get(request);
            var users = (List<UserResponseDTO>?)response.Value;
            Assert.AreEqual(users!.Count, 1);
        }

        [TestMethod("Should be able get all")]
        public async Task GetAll()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _repository.Setup(x => x.Get()).ReturnsAsync([entity]);

            var service = CreateService();
            var request = new UserGetRequestDTO();
            var response = await service.Get(request);
            var users = (List<UserResponseDTO>?)response.Value;
            Assert.AreEqual(users!.Count, 1);
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            _repository.Setup(x => x.Remove(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new UserRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able remove")]
        public async Task NotRemove()
        {
            _repository.Setup(x => x.Remove(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able update")]
        public async Task Update()
        {
            _repository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new UserUpdateRequestDTO
            {
                Id = Guid.NewGuid(),
                Language = UserLanguageConstant.EN_US,
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able update")]
        public async Task NotUpdate()
        {
            _repository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync(true);
            _repository.Setup(x => x.Exists(It.IsAny<User>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new UserUpdateRequestDTO
            {
                Id = Guid.NewGuid(),
                Language = UserLanguageConstant.EN_US,
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }
    }
}
