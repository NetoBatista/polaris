using Polaris.Domain.Dto.Application;
using Polaris.Domain.Interface.Service;
using Polaris.Extension;
using Microsoft.AspNetCore.Mvc;

namespace Polaris.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : BaseControllerV1Extension
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationResponseDTO>> Create([FromBody] ApplicationCreateRequestDTO request)
        {
            var response = await _applicationService.Create(request);
            return ToObjectResult(response);
        }

        [HttpPatch("{applicationId}")]
        public async Task<ActionResult<ApplicationResponseDTO>> Update(Guid applicationId, [FromBody] ApplicationUpdateRequestDTO request)
        {
            request.Id = applicationId;
            var response = await _applicationService.Update(request);
            return ToObjectResult(response);
        }

        [HttpDelete("{applicationId}")]
        public async Task<ActionResult> Remove(Guid applicationId)
        {
            var request = new ApplicationRemoveRequestDTO
            {
                Id = applicationId
            };
            var response = await _applicationService.Remove(request);
            return ToObjectResult(response);
        }

        [HttpGet]
        public async Task<ActionResult<ApplicationResponseDTO>> Get()
        {
            var response = await _applicationService.GetAll();
            return ToObjectResult(response);
        }
    }
}
