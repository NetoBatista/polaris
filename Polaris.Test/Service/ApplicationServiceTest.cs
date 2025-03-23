using Moq;
using Polaris.Domain.Dto.Application;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Validator.Application;
using Polaris.Service;
using System.Net;

namespace Polaris.Test.Service
{
    [TestClass]
    public class ApplicationServiceTest
    {
        private Mock<IApplicationRepository> _repository = new Mock<IApplicationRepository>();

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IApplicationRepository>();
        }

        [TestCleanup]
        public void Teardown()
        {
            _repository.Reset();
        }

        private ApplicationService CreateService()
        {
            var createValidator = new ApplicationCreateValidator(_repository.Object);
            var updateValidator = new ApplicationUpdateValidator(_repository.Object);
            var removeValidator = new ApplicationRemoveValidator(_repository.Object);

            return new ApplicationService(_repository.Object, createValidator, updateValidator, removeValidator);
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.Create(It.IsAny<Application>())).ReturnsAsync(application);

            var service = CreateService();
            var request = new ApplicationCreateRequestDTO
            {
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able create already created")]
        public async Task NotCreateAlreadyCreated()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new ApplicationCreateRequestDTO
            {
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create name empty")]
        public async Task NotCreateNameEmpty()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new ApplicationCreateRequestDTO
            {
                Name = string.Empty
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able create name less then 3")]
        public async Task NotCreateNameLessThen3()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new ApplicationCreateRequestDTO
            {
                Name = "ab"
            };
            var response = await service.Create(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able update application")]
        public async Task Update()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.Update(It.IsAny<Application>())).ReturnsAsync(application);

            var service = CreateService();
            var request = new ApplicationUpdateRequestDTO
            {
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able update application name empty")]
        public async Task NotUpdateNameEmpty()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new ApplicationUpdateRequestDTO
            {
                Name = string.Empty
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able update application name less then 3")]
        public async Task NotUpdateNameLessThen3()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new ApplicationUpdateRequestDTO
            {
                Name = "ab"
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able update application name more then 256")]
        public async Task NotUpdateEmptyMoreThen256()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var name = string.Empty;
            while (name.Length >= 256)
            {
                name += Guid.NewGuid().ToString();
            }
            var request = new ApplicationUpdateRequestDTO
            {
                Name = name
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able update application name already exists")]
        public async Task NotUpdateNameAlreadyExists()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new ApplicationUpdateRequestDTO
            {
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able update application id not found")]
        public async Task NotUpdateIdNotFound()
        {
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.NameAlreadyExists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new ApplicationUpdateRequestDTO
            {
                Name = Guid.NewGuid().ToString()
            };
            var response = await service.Update(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            _repository.Setup(x => x.AnyMember(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new ApplicationRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should not be able remove id not found")]
        public async Task NotRemoveExists()
        {
            _repository.Setup(x => x.AnyMember(It.IsAny<Application>())).ReturnsAsync(false);
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(false);

            var service = CreateService();
            var request = new ApplicationRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should not be able remove has members")]
        public async Task NotRemoveHasMembers()
        {
            _repository.Setup(x => x.AnyMember(It.IsAny<Application>())).ReturnsAsync(true);
            _repository.Setup(x => x.Exists(It.IsAny<Application>())).ReturnsAsync(true);

            var service = CreateService();
            var request = new ApplicationRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };
            var response = await service.Remove(request);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod("Should be able get all")]
        public async Task GetAll()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            _repository.Setup(x => x.GetAll()).ReturnsAsync([application]);

            var service = CreateService();
            var response = await service.GetAll();
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}
