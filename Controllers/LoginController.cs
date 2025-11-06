using ASKPA.API.BLL;
using ASKPA.API.Common;
using ASKPA.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ASKPA_API.Controllers.Admin
{
    [Route("api/crm")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string AdminConnectionString;
        public LoginController(IConfiguration configuration)
        {
            AdminConnectionString = configuration.GetConnectionString("DBAdmin");
        }



        //private readonly string _privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "private.key");

        //[HttpPost]
        //[Route("Login/ValidateLogin")]
        //public async Task<IActionResult> Login_Check(EncryptedLoginID info)
        //{
        //    try
        //    {
        //        if (!System.IO.File.Exists(_privateKeyPath))
        //            return NotFound("Private key not found.");

        //        var privateKeyPem = System.IO.File.ReadAllText(_privateKeyPath);
        //        using RSA rsa = RSA.Create();
        //        rsa.ImportFromPem(privateKeyPem.ToCharArray());

        //        string decryptedUserId = clsHelperDBDecrypt.DecryptWithRSA(info.EncryptedUserId, rsa);
        //        string decryptedPassword = clsHelperDBDecrypt.DecryptWithRSA(info.EncryptedPassword, rsa);
        //        string AdminConnection = HttpContext.Items["AdminConnection"] as string;
        //        var loginInfo = new clsLoginInfo
        //        {
        //            UserID = decryptedUserId,
        //            ColumnData = decryptedPassword
        //        };
        //        clsLoginData obj = await clsLogin.LoginCheckAsync(AdminConnection, loginInfo);
        //        return Ok(obj);
        //    }
        //    catch (CryptographicException ex)
        //    {
        //        return BadRequest(new { error = $"Decryption failed: {ex.Message}" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
        //    }
        //}

        [HttpPost]
        [Route("Login/ValidateLogin")]
        public async Task<IActionResult> Login_Validation(clsLoginInfo info)
        {
            clsLoginResultInfo d = await clsLogin.Login_Validation(AdminConnectionString, info);
            return Ok(new { data = d });
        }
    }
}
