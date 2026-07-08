using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Application.Interfaces.Services
{
    public interface IModelsCharacterService
    {
        Task<ApiResponse<IEnumerable<ModelCharacterResponse>>> GetAllAsync(int pageIndex, int pageSize, string search);
    }
}
