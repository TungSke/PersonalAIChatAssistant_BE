using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Swashbuckle.AspNetCore.Annotations;
using PersonalAIAssistant.Application.Interfaces.Services;
using PersonalAIAssistant.Application.DTOs.Request;

namespace PersonalAIAssistant.API.Controllers
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
        [EndpointSummary("Get all characters that the user has not chatted with.")]
        public async Task<IActionResult> Get(int pageIndex = 1, int pageSize = 5, string? search = "")
        {
            var characters = await _modelsCharacterService.GetAllAsync(pageIndex, pageSize, search);
            if (characters == null)
            {
                return NotFound("No characters found.");
            }
            return Ok(characters);
        }

        [HttpPost]
        [EndpointSummary("Create a new character.")]
        public async Task<IActionResult> Create([FromBody] ModelCharacterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body is null.");
            }
            var createdCharacter = await _modelsCharacterService.CreateAsync(request);
            if (createdCharacter == null)
            {
                return BadRequest("Failed to create character.");
            }
            return CreatedAtAction(nameof(Get), new { id = createdCharacter.Data?.Id }, createdCharacter);
        }

        [HttpPut("{id}")]
        [EndpointSummary("Update an existing character.")]
        public async Task<IActionResult> Update(int id, [FromBody] ModelCharacterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body is null.");
            }
            var updatedCharacter = await _modelsCharacterService.UpdateAsync(id, request);
            if (updatedCharacter == null)
            {
                return NotFound($"Character with ID {id} not found.");
            }
            return Ok(updatedCharacter);
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Delete a character.")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedCharacter = await _modelsCharacterService.DeleteAsync(id);
            if (deletedCharacter == null)
            {
                return NotFound($"Character with ID {id} not found.");
            }
            return Ok(deletedCharacter);
        }
    }
}
