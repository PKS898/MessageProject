using Microsoft.AspNetCore.Mvc;
using MessageProject.DAL;
using MessageProject.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MessageProject.Models;

namespace MessageProject.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly MessageRepository _repository;
        private readonly IHubContext<MessageHub> _hubContext;

        private readonly ILogger<MessageController> _logger;

        public MessageController(MessageRepository repository, IHubContext<MessageHub> hubContext, ILogger<MessageController> logger)
        {
            _repository = repository;
            _hubContext = hubContext;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var messages = await _repository.GetMessagesAsync(from, to);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Text) || message.Text.Length > 128)
            {
                return BadRequest(new { error = "Message text must be between 1 and 128 characters" });
            }

            message.Timestamp = DateTime.UtcNow;
            var success = await _repository.InsertMessageAsync(message);

            if (success)
            {
                _logger.LogInformation("📡 Broadcasting message: {MessageText}", message.Text);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.SequenceNumber, message.Text, message.Timestamp);
                return Ok(new { message = "Message sent successfully" });
            }

            return BadRequest(new { error = "Failed to insert message" });
        }

    }
}
