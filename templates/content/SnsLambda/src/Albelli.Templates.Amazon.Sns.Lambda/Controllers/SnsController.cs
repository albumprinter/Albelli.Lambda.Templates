using System.Threading.Tasks;
using Albelli.Templates.Amazon.Sns.Lambda.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albelli.Templates.Amazon.Sns.Lambda.Controllers
{
    [Route("api/[controller]")]
    public sealed class SnsController : Controller
    {
        private readonly ILogger<SnsController> _logger;

        public SnsController(
            ILogger<SnsController> logger
        )
        {
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Post([FromBody] NotificationDto notification)
        {
            _logger.LogDebug("Got event {Message}", notification?.Message);
            return Ok();
        }
    }
}