using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.Interfaces.Services;
using System.ComponentModel;
using PersonalAIAssistant.Application.DTOs.Response;

namespace PersonalAIAssistant.API.Controllers
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
        [EndpointSummary("Get messages in a conversation")]
        [EndpointDescription("Supports initial load, loading older messages (beforeMessageId), and fetching new messages")]
        public async Task<IActionResult> GetMessages(
            [FromQuery][Required]
            [Description("The ID of the conversation to retrieve messages from")]
            int conversationId,

            [FromQuery]
            int limit = 30,

            [FromQuery]
            [Description("Fetch messages with IDs less than this value (used for loading older messages when scrolling up)")]
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
        [EndpointSummary("Create a new message in a conversation")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MessageResponse))]
        public async Task<IActionResult> CreateMessage(MessageRequest request)
        {
            var response = await _service.CreateMessage(request);
            if (response.Success == true)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
