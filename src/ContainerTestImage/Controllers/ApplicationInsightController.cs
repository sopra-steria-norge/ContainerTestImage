using Microsoft.AspNetCore.Mvc;
using System;

namespace ContainerTestImage.Controllers
{
    [ApiController]
    public class ApplicationInsightController : ControllerBase
    {
        public ApplicationInsightController()
        {
        }

        [HttpGet("TestThrowException")]
        public void TestThrowException()
        {
            throw new Exception("Testing Application Insight");
        }

        
        //[HttpGet("TestThrowException")]
        //public void TestThrowException()
        //{
        //    throw new Exception("Testing Application Insight");
        //}
    }
}
