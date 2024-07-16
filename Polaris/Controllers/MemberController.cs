using Polaris.Domain.Dto.Member;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Interface.Service;
using Polaris.Extension;
using Microsoft.AspNetCore.Mvc;

namespace Polaris.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController : BaseControllerV1Extension
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] MemberCreateRequestDTO request)
        {
            var response = await _memberService.Create(request);
            return ToObjectResult(response);
        }

        [HttpGet("{applicationId}/Application")]
        public async Task<ActionResult<UserResponseDTO>> GetByApplication(Guid applicationId)
        {
            var request = new MemberGetApplicationRequestDTO { ApplicationId = applicationId };
            var response = await _memberService.GetByApplication(request);
            return ToObjectResult(response);
        }

        [HttpGet("{userId}/User")]
        public async Task<ActionResult<UserResponseDTO>> GetByUser(Guid userId)
        {
            var request = new MemberGetUserRequestDTO { UserId = userId };
            var response = await _memberService.GetByUser(request);
            return ToObjectResult(response);
        }

        [HttpDelete("{memberId}")]
        public async Task<ActionResult> Remove(Guid memberId)
        {
            var request = new MemberRemoveRequestDTO
            {
                Id = memberId
            };
            var response = await _memberService.Remove(request);
            return ToObjectResult(response);
        }
    }
}
