using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.Interfaces;

namespace WaifuAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;
        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpGet("conversationId")]
        public async Task<IActionResult> GetAllMessageInConversation(int conversationId)
        {
            var response = await _service.GetMessagesFromConversation(conversationId);
            return Ok(response);
        }

        [HttpPost]
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
