using ASKPA_API.BLL;
using ASKPA_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASKPA_API.Controllers
{
    [Route("askpa/admin")]
    [ApiController]
    public class MiscController : ControllerBase
    {
        [HttpGet("Misc/MiscList")]
        public async Task<IActionResult> Misc_List(string type)
        {
            string businessConnection = HttpContext.Items["CompanyConnection"] as string;
            var d = await clsMisc.Misc_List(businessConnection, type);
            return Ok(new { data= d});
        }
        [HttpPost]
        [Route("Misc/Save")]
        public async Task<IActionResult> Misc_Save(MiscInfo info)
        {
            string connection = HttpContext.Items["CompanyConnection"] as string;
            var d = await clsMisc.Misc_Add_Edit(connection, info);
            return Ok(new { data = d });
        }
        [HttpGet("Misc/List")]
        public async Task<IActionResult> MiscInfo_List()
        {
            string businessConnection = HttpContext.Items["CompanyConnection"] as string;
            var d = await clsMisc.MiscInfo_List(businessConnection);
            return Ok(new { data = d });
        }
        [HttpGet("Misc/Detail")]
        public async Task<IActionResult> MiscInfo_Details(long MiscID)
        {
            string businessConnection = HttpContext.Items["CompanyConnection"] as string;
            var d = await clsMisc.Misc_Detail(businessConnection, MiscID);
            return Ok(new { data = d });
        }
    }
}
