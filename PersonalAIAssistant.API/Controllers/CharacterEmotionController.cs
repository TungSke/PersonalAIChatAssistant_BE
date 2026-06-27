using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using PersonalAIAssistant.Application.Interfaces;

namespace PersonalAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CharacterEmotionController : ControllerBase
    {
        private readonly ICharacterEmotionService _characterEmotionService;
        public CharacterEmotionController(ICharacterEmotionService characterEmotionService)
        {
            _characterEmotionService = characterEmotionService;
        }

        [SwaggerOperation(
        Summary = "Get all character emotion need when create"
        )]
        [HttpGet("get-character-emotions")]
        public async Task<IActionResult> GetAllDataEmotion()
        {
            var result = await _characterEmotionService.GetAllDataEmotion();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int characterId)
        {
            var result = await _characterEmotionService.GetCharacterEmotion(characterId);
            if (result == null)
            {
                return NotFound("Character dont have any emotions");
            }
            return Ok(result);
        }
    }
}
