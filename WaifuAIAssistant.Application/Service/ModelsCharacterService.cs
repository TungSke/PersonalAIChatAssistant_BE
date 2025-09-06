using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Application.Service
{
    public class ModelsCharacterService : IModelsCharacterService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ModelsCharacterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse<IEnumerable<ModelsCharacter>>> GetAllAsync(int pageIndex, int pageSize, string search)
        {
            var list = await _unitOfWork.ModelRepository.GetPagedAsync(pageIndex, pageSize,
                x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search))
                );
            if (list == null || !list.Any())
            {
                return new ApiResponse<IEnumerable<ModelsCharacter>>
                {
                    Success = false,
                    Message = "No models found."
                };
            }

            return new ApiResponse<IEnumerable<ModelsCharacter>>
            {
                Success = true,
                Data = list
            };
        }
    }
}
