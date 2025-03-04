using Microsoft.AspNetCore.Mvc;
using Moq;
using Polaris.Controllers;
using Polaris.Domain.Dto.Member;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Model;
using System.Net;

namespace Polaris.Test.Controller
{
    [TestClass]
    public class MemberControllerTest
    {
        private Mock<IMemberService> _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new Mock<IMemberService>();
        }

        private MemberController CreateController()
        {
            return new MemberController(_service.Object);
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var request = new MemberCreateRequestDTO();
            var responseBase = ResponseBaseModel.Ok();

            _service.Setup(service => service.Create(request))
                              .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Create(request);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able GetByApplication")]
        public async Task GetByApplication()
        {

            var applicationId = Guid.NewGuid();
            var request = new MemberGetApplicationRequestDTO
            {
                ApplicationId = applicationId
            };
            var responseDTO = new MemberApplicationResponseDTO
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Users = new List<MemberItemUserResponseDTO>
                {
                    new MemberItemUserResponseDTO
                    {
                        Email = $"{Guid.NewGuid()}@email.com",
                        MemberId = Guid.NewGuid()
                    }
                }
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(service => service.GetByApplication(It.IsAny<MemberGetApplicationRequestDTO>()))
                              .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.GetByApplication(applicationId);

            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able GetByUser")]
        public async Task GetByUser()
        {

            var userId = Guid.NewGuid();
            var request = new MemberGetUserRequestDTO
            {
                UserId = userId
            };
            var responseDTO = new MemberApplicationResponseDTO
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Users = new List<MemberItemUserResponseDTO>
                {
                    new MemberItemUserResponseDTO
                    {
                        Email= $"{Guid.NewGuid()}@email.com",
                        MemberId = Guid.NewGuid()
                    }
                }
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(service => service.GetByUser(It.IsAny<MemberGetUserRequestDTO>()))
                              .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.GetByUser(userId);
            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able Remove")]
        public async Task Remove()
        {
            var memberId = Guid.NewGuid();
            var request = new MemberRemoveRequestDTO
            {
                Id = memberId
            };
            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(service => service.Remove(It.IsAny<MemberRemoveRequestDTO>()))
                              .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Remove(memberId);

            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}
