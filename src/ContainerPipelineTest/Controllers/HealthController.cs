using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ContainerPipelineTest.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {

        public HealthController()
        {
        }

        /// <summary>
        /// Health endpoint to check if the service is running. Required for the API gateway to work.
        /// </summary>
        /// <returns></returns>
        [HttpGet("health")]
        public string Health()
        {
            return "OK";
        }

        /// <summary>
        /// Health endpoint to check if the service is running. Required for the API gateway to work.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        public string HealthRoot()
        {
            return "OK";
        }

        
        [Authorize]
        [HttpGet("health/authorize")]
        public string HealthAuthorize()
        {
            return "OK";
        }        
        
    }
}
