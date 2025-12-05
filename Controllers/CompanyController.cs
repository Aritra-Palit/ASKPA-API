using ASKPA_API.BLL;
using ASKPA_API.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Route("company/list")]
        public async Task<IActionResult> Company_List()
        {
            string businessConnection = HttpContext.Items["CompanyConnection"] as string;
            List<clsCompanyList> d = await clsCompany.Company_List(businessConnection);
            return Ok(new { data = d });
        }
        [HttpGet]
        [Route("company/config")]
        public async Task<IActionResult> Config_List(int IDCompany)
        {
            string businessConnection = HttpContext.Items["CompanyConnection"] as string;
            List<clsConfigList> d = await clsCompany.Config_List(businessConnection, IDCompany);
            return Ok(new { data = d });
        }


    }
}
