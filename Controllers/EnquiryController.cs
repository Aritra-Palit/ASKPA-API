using ASKPA_API.BLL;
using ASKPA_API.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
namespace ASKPA_API.Controllers
{
    [Route("askpa/admin")]
    [ApiController]
    public class EnquiryController : ControllerBase
    {
        [HttpPost]
        [Route("enquiry/new")]
        public async Task<IActionResult> NewEnquiryDetails(clsEnquiryDetails info)
        {
            string connection = HttpContext.Items["AdminConnection"] as string;
            var no = await clsEnquiry.NewEnquiry(connection, info);
            return Ok(new { EnquiryNumber = no });
        }
    }
}
