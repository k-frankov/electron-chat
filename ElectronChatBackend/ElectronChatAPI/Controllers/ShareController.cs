using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectronChatAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShareController : ControllerBase
    {
        private readonly ILogger<ShareController> logger;

        public ShareController(ILogger<ShareController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ShareFile(IFormFile file)
        {
            var mb = (file.Length / 1024f) / 1024f;
            if (mb <= 0 ||mb > 5)
            {
                return BadRequest("File is too big");
            }
            return Ok();
        }
    }
}
