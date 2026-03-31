using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.Interfaces;

namespace WaifuAIAssistant.API.Controllers
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

        private string getUserid() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet]
        public async Task<IActionResult> GetAllConversation()
        {
            var response = await _service.GetConversationAsync();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation(ConversationRequest request)
        {
            var userId = getUserid();
            var reponse = await _service.CreateConversation(request);
            if (reponse.Success == false)
            {
                return BadRequest(Response);
            }
            return Ok(reponse);
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
