using Microsoft.AspNetCore.Mvc;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Interface.Service;
using Polaris.Extension;

namespace Polaris.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseControllerV1Extension
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create([FromBody] UserCreateRequestDTO request)
        {
            var response = await _userService.Create(request);
            return ToObjectResult(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponseDTO>>> Get([FromQuery] UserGetRequestDTO request)
        {
            var response = await _userService.Get(request);
            return ToObjectResult(response);
        }

        [HttpPatch("{userId}")]
        public async Task<ActionResult> Update(Guid userId, [FromBody] UserUpdateRequestDTO request)
        {
            request.Id = userId;
            var response = await _userService.Update(request);
            return ToObjectResult(response);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Remove(Guid userId)
        {
            var request = new UserRemoveRequestDTO
            {
                Id = userId
            };
            var response = await _userService.Remove(request);
            return ToObjectResult(response);
        }
    }
}
