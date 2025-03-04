﻿using Microsoft.AspNetCore.Mvc;
using Polaris.Domain.Model;

namespace Polaris.Extension
{
    [ApiController]
    [Route("v1/[controller]")]
    public class BaseControllerV1Extension : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult ToObjectResult(ResponseBaseModel response)
        {
            return StatusCode(response.StatusCode, response.Value);
        }
    }
}
