using Microsoft.AspNetCore.Mvc;
using Moq;
using Polaris.Controllers;
using Polaris.Domain.Constant;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Model;
using System.Net;

namespace Polaris.Test.Controller
{
    [TestClass]
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _service;
        public UserControllerTest()
        {
            _service = new Mock<IUserService>();
        }

        private UserController CreateController()
        {
            return new UserController(_service.Object);
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var request = new UserCreateRequestDTO
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Language = UserLanguageConstant.EN_US,
                Name = Guid.NewGuid().ToString()
            };

            var responseDTO = new UserResponseDTO
            {
                Email = request.Email,
                Id = Guid.NewGuid(),
                Language = request.Language,
                Name = request.Name
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(x => x.Create(It.IsAny<UserCreateRequestDTO>())).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Create(request);
            Assert.IsNull(response.Value);
        }

        [TestMethod("Should be able get")]
        public async Task Get()
        {
            var request = new UserGetRequestDTO
            {
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var responseDTO = new UserResponseDTO
            {
                Email = request.Email,
                Id = Guid.NewGuid(),
                Language = UserLanguageConstant.EN_US,
                Name = Guid.NewGuid().ToString()
            };
            var responseBase = ResponseBaseModel.Ok(new List<UserResponseDTO> { responseDTO });
            _service.Setup(x => x.Get(It.IsAny<UserGetRequestDTO>())).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Get(request);
            Assert.IsNull(response.Value);
        }

        [TestMethod("Should be able patch")]
        public async Task Patch()
        {
            var request = new UserUpdateRequestDTO
            {
                Id = Guid.NewGuid(),
                Language = UserLanguageConstant.EN_US,
                Name = Guid.NewGuid().ToString()
            };

            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(x => x.Update(It.IsAny<UserUpdateRequestDTO>())).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Update(request.Id, request);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able Remove")]
        public async Task Remove()
        {
            var request = new UserRemoveRequestDTO
            {
                Id = Guid.NewGuid()
            };

            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(x => x.Remove(It.IsAny<UserRemoveRequestDTO>())).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Remove(request.Id);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}
