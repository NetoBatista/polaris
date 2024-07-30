using Microsoft.AspNetCore.Mvc;
using Moq;
using Polaris.Controllers;
using Polaris.Domain.Dto.Application;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Model;
using System.Net;

namespace Polaris.Test.Controller
{
    [TestClass]
    public class ApplicationControllerTest
    {
        private readonly Mock<IApplicationService> _service;
        public ApplicationControllerTest()
        {
            _service = new Mock<IApplicationService>();
        }

        private ApplicationController CreateController()
        {
            return new ApplicationController(_service.Object);
        }

        [TestMethod("Should be able to create application")]
        public async Task Create()
        {
            var request = new ApplicationCreateRequestDTO
            {
                Name = Guid.NewGuid().ToString()
            };

            var responseDTO = new ApplicationResponseDTO
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(x => x.Create(It.IsAny<ApplicationCreateRequestDTO>()))
                    .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Create(request);
            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to update application")]
        public async Task Update()
        {
            var applicationId = Guid.NewGuid();
            var request = new ApplicationUpdateRequestDTO
            {
                Name = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid()
            };

            var responseDTO = new ApplicationResponseDTO
            {
                Id = applicationId,
                Name = request.Name
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(x => x.Update(It.IsAny<ApplicationUpdateRequestDTO>()))
                    .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Update(applicationId, request);
            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to remove application")]
        public async Task Remove()
        {
            var applicationId = Guid.NewGuid();

            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(x => x.Remove(It.IsAny<ApplicationRemoveRequestDTO>()))
                    .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Remove(applicationId);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to get all applications")]
        public async Task GetAll()
        {
            var responseDTOs = new List<ApplicationResponseDTO>
            {
                new ApplicationResponseDTO
                {
                    Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString()
                },
                new ApplicationResponseDTO
                {
                    Id = Guid.NewGuid(), Name =  Guid.NewGuid().ToString()
                }
            };
            var responseBase = ResponseBaseModel.Ok(responseDTOs);
            _service.Setup(x => x.GetAll()).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Get();
            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}
