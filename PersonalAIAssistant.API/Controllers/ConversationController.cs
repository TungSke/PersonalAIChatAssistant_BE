using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.Interfaces.Services;

namespace PersonalAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _service;
        public ConversationController(IConversationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversation()
        {
            var response = await _service.GetConversationAsync();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation(ConversationRequest request)
        {
            var reponse = await _service.CreateConversation(request);
            if (reponse.Success == false)
            {
                return BadRequest(reponse);
            }
            return CreatedAtAction(nameof(GetAllConversation), reponse);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteConversation(int conversationId)
        {
            var response = await _service.DeleteConversation(conversationId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
