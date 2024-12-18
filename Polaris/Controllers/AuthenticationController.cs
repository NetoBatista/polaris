using Microsoft.AspNetCore.Mvc;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Interface.Service;
using Polaris.Extension;

namespace Polaris.Controllers
{
    public class AuthenticationController : BaseControllerV1Extension
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthenticationResponseDTO>> Authenticate([FromBody] AuthenticationRequestDTO request)
        {
            var response = await _authenticationService.Authenticate(request);
            return ToObjectResult(response);
        }
        
        [HttpPost("Firebase")]
        public async Task<ActionResult<AuthenticationResponseDTO>> AuthenticateFirebase([FromBody] AuthenticationFirebaseRequestDTO request)
        {
            var response = await _authenticationService.AuthenticateFirebase(request);
            return ToObjectResult(response);
        }

        [HttpPost("Code")]
        public async Task<ActionResult> GenerateCode([FromBody] AuthenticationGenerateCodeRequestDTO request)
        {
            var response = await _authenticationService.GenerateCode(request);
            return ToObjectResult(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthenticationResponseDTO>> RefreshToken([FromBody] AuthenticationRefreshTokenRequestDTO request)
        {
            var response = await _authenticationService.RefreshToken(request);
            return ToObjectResult(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] AuthenticationChangePasswordRequestDTO request)
        {
            var response = await _authenticationService.ChangePassword(request);
            return ToObjectResult(response);
        }
    }
}
