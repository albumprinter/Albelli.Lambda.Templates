using System.Threading.Tasks;
using Albelli.Templates.Amazon.Sqs.Lambda.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albelli.Templates.Amazon.Sqs.Lambda.Controllers
{
    [Route("api/[controller]")]
    public sealed class SqsController : Controller
    {
        private readonly ILogger<SqsController> _logger;

        public SqsController(
            ILogger<SqsController> logger
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