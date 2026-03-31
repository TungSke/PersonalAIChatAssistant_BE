using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using WaifuAIAssistant.Application.Interfaces;

namespace WaifuAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModelsCharacterController : ControllerBase
    {
        private readonly IModelsCharacterService _modelsCharacterService;
        public ModelsCharacterController(IModelsCharacterService modelsCharacterService)
        {
            _modelsCharacterService = modelsCharacterService ?? throw new ArgumentNullException(nameof(modelsCharacterService));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageIndex = 1, int pageSize = 5, string? search = "")
        {
            var characters = await _modelsCharacterService.GetAllAsync(pageIndex, pageSize, search);
            if (characters == null)
            {
                return NotFound("No characters found.");
            }
            return Ok(characters);
        }
    }
}
