using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Application.Interfaces
{
    public interface IModelsCharacterService
    {
        Task<ApiResponse<IEnumerable<ModelCharacterResponse>>> GetAllAsync(int pageIndex, int pageSize, string search);
    }
}
