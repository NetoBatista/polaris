using Microsoft.AspNetCore.Mvc;
using Polaris.Extension;

namespace Polaris.Controllers
{
    public class PingController : BaseControllerV1Extension
    {
        
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }
    }
}
