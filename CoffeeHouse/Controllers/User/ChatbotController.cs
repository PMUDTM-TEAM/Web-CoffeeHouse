using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;

namespace CoffeeHouse.Controllers.User
{
    [Route("api/messages")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IBot _bot;
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(IBotFrameworkHttpAdapter adapter, IBot bot, ILogger<ChatbotController> logger)
        {
            _adapter = adapter;
            _bot = bot;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                _logger.LogInformation("Received a POST request at /api/messages");
                await _adapter.ProcessAsync(Request, Response, _bot);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing the request in ChatbotController");
                return StatusCode(500, "Error processing the request.");
            }
        }
    }


}
