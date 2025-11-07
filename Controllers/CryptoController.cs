using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ASKPA_API.Controllers
{
    [Route("askpa/admin")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly string _publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Keys", "public.key");
        [HttpGet]
        [Route("crypto/encrypt")]
        public IActionResult GetPublicKey()
        {
            if (!System.IO.File.Exists(_publicKeyPath))
                return NotFound("Public key not found.");
            var publicKeyPem = System.IO.File.ReadAllText(_publicKeyPath);
            return Content(publicKeyPem, "text/plain");
        }
    }
}
