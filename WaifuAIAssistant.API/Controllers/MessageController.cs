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

        [HttpGet("{conversationId}/{beforeMessageId}")]
        public async Task<IActionResult> GetAllMessageInConversation(
            [Required]int conversationId,
            [SwaggerParameter("The ID of the message to start before")] int? beforeMessageId)
        {
            var response = await _service.GetMessagesFromConversation(conversationId, 30, beforeMessageId);
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
