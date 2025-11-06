using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;

namespace ASKPA_API.Controllers
{
    [Route("api/crm")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public IConfiguration AdminConfiguration { get; set; }
        public TestController(IConfiguration configuration)
        {
            AdminConfiguration = configuration;
        }
        [HttpGet]
        [Route("demo")]
        public IActionResult Demo()
        {
            string strAdminConn = AdminConfiguration.GetConnectionString("DBAdmin").ToString();
            var Message = "Welcome to ASKPA-API:" + Environment.NewLine;
            Message += "API Version:2025-01-01.001" + Environment.NewLine;
            Message += "OS Description: " + System.Runtime.InteropServices.RuntimeInformation.OSDescription + Environment.NewLine;
            Message += "OS Architecture: " + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture + Environment.NewLine;
            Message += "Computer Name: " + Environment.MachineName + Environment.NewLine;
            Message += "OS Version: " + Environment.OSVersion.ToString() + Environment.NewLine;
            Message += "Computer Processor: " + Environment.ProcessorCount + Environment.NewLine;
            Message += "Net Framwork Version: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription + Environment.NewLine;
            Message += "App Directory: " + Environment.CurrentDirectory + Environment.NewLine;
            Message += "Date and Time:" + DateTime.Now.ToString() + Environment.NewLine;
            return Ok(Message);
        }
        


    }
}
