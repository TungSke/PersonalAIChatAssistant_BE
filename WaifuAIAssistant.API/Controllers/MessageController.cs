using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.Interfaces;

namespace WaifuAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("messagePolicy")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;
        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(
    Summary = "Get messages in a conversation",
    Description = "Supports initial load, loading older messages (beforeMessageId), and fetching new messages (afterMessageId)."
)]
        public async Task<IActionResult> GetMessages(
    [FromQuery][Required]
    [SwaggerParameter("The ID of the conversation to retrieve messages from", Required = true)]
    int conversationId,

    [FromQuery]
    int limit = 30,

    [FromQuery]
    [SwaggerParameter("Fetch messages with IDs less than this value (used for loading older messages when scrolling up)")]
    long? beforeMessageId = null)
        {
            var response = await _service.GetMessagesFromConversation(
                conversationId,
                limit,
                beforeMessageId
            );

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new message in a conversation")]
        public async Task<IActionResult> CreateMessage(MessageRequest request)
        {
            var response = await _service.CreateMessage(request);
            if(response.Success == true)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
