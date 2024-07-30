using Microsoft.AspNetCore.Mvc;
using Moq;
using Polaris.Controllers;
using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Model;
using System.Net;

namespace Polaris.Test.Controller
{
    [TestClass]
    public class AuthenticationControllerTest
    {
        private readonly Mock<IAuthenticationService> _service;
        public AuthenticationControllerTest()
        {
            _service = new Mock<IAuthenticationService>();
        }

        private AuthenticationController CreateController()
        {
            return new AuthenticationController(_service.Object);
        }

        [TestMethod("Should be able to authenticate")]
        public async Task Authenticate()
        {
            var request = new AuthenticationRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Code = "123456",
                Password = "123456",
                Email = $"{Guid.NewGuid()}@email.com"
            };

            var responseDTO = new AuthenticationResponseDTO
            {
                Token = "token",
                Expire = 5,
                RefreshToken = Guid.NewGuid().ToString(),
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(x => x.Authenticate(It.IsAny<AuthenticationRequestDTO>())).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.Authenticate(request);
            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to generate code")]
        public async Task GenerateCode()
        {
            var request = new AuthenticationGenerateCodeRequestDTO
            {
                Email = $"{Guid.NewGuid()}@email.com",
                ApplicationId = Guid.NewGuid()
            };

            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(x => x.GenerateCode(It.IsAny<AuthenticationGenerateCodeRequestDTO>()))
                    .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.GenerateCode(request);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to refresh token")]
        public async Task RefreshToken()
        {
            var request = new AuthenticationRefreshTokenRequestDTO
            {
                RefreshToken = Guid.NewGuid()
            };

            var responseDTO = new AuthenticationResponseDTO
            {
                Token = "newToken",
                Expire = 5,
                RefreshToken = Guid.NewGuid().ToString()
            };
            var responseBase = ResponseBaseModel.Ok(responseDTO);
            _service.Setup(x => x.RefreshToken(It.IsAny<AuthenticationRefreshTokenRequestDTO>())).ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.RefreshToken(request);
            var result = (ObjectResult?)response.Result;
            Assert.AreEqual(result!.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to change type")]
        public async Task ChangeType()
        {
            var request = new AuthenticationChangeTypeRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                Email = $"{Guid.NewGuid()}@email.com",
                Password = "123456",
                Type = AuthenticationTypeConstant.EmailOnly
            };

            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(x => x.ChangeType(It.IsAny<AuthenticationChangeTypeRequestDTO>()))
                    .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.ChangeType(request);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [TestMethod("Should be able to change password")]
        public async Task ChangePassword()
        {
            var request = new AuthenticationChangePasswordRequestDTO
            {
                ApplicationId = Guid.NewGuid(),
                CurrentPassword = "123456",
                Email = $"{Guid.NewGuid()}@email.com",
                Password = "123456"
            };

            var responseBase = ResponseBaseModel.Ok();
            _service.Setup(x => x.ChangePassword(It.IsAny<AuthenticationChangePasswordRequestDTO>()))
                    .ReturnsAsync(responseBase);

            var controller = CreateController();
            var response = await controller.ChangePassword(request);
            var result = (ObjectResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}
