using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaifuAIAssistant.Application.Interfaces;

namespace WaifuAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterEmotionController : ControllerBase
    {
        private readonly ICharacterEmotionService _characterEmotionService;
        public CharacterEmotionController(ICharacterEmotionService characterEmotionService)
        {
            _characterEmotionService = characterEmotionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int characterId)
        {
            var result = await _characterEmotionService.GetCharacterEmotion(characterId);
            if(result == null)
            {
                return NotFound("Character dont have any emotions");
            }
            return Ok(result);
        }
    }
}
