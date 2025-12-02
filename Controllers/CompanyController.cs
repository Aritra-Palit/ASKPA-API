using ASKPA_API.BLL;
using ASKPA_API.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
namespace ASKPA_API.Controllers
{
    [Route("askpa/admin")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        [HttpPost]
        [Route("company/new")]
        public async Task<IActionResult> NewCompanyDetails(clsCompanyInfo info)
        {
            string connection = HttpContext.Items["CompanyConnection"] as string;
            var no = await clsCompany.NewCompany(connection, info);
            return Ok(new { MerchantNumber = no });
        }
    }
}
