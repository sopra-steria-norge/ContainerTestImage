using Microsoft.AspNetCore.Mvc;
using System;

namespace ContainerPipelineTest.Controllers
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
