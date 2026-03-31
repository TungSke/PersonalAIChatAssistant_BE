using Mapster;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Application.Service
{
    public class ModelsCharacterService : IModelsCharacterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;
        public ModelsCharacterService(IUnitOfWork unitOfWork, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task<ApiResponse<IEnumerable<ModelCharacterResponse>>> GetAllAsync(int pageIndex, int pageSize, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                var cacheKey = "model_characters:all";

                var cachedData = await _redisCacheService
                    .GetAsync<List<ModelCharacterResponse>>(cacheKey);

                if (cachedData != null)
                {
                    var pagedData = cachedData
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    return new ApiResponse<IEnumerable<ModelCharacterResponse>>
                    {
                        Success = true,
                        Data = pagedData
                    };
                }
            }

            var list = await _unitOfWork.ModelRepository.GetPagedAsync(
                            pageIndex,
                            pageSize,
                            x => string.IsNullOrEmpty(search) ||
                                 x.Name.ToLower().Contains(search.ToLower())
    );

            if (list == null || !list.Any())
            {
                throw new KeyNotFoundException("No model characters found.");
            }

            var response = list.Adapt<List<ModelCharacterResponse>>();

            if (string.IsNullOrEmpty(search))
            {
                await _redisCacheService.SetAsync(
                    "model_characters:all",
                    response,
                    TimeSpan.FromMinutes(5)
                );
            }

            return new ApiResponse<IEnumerable<ModelCharacterResponse>>
            {
                Success = true,
                Data = response
            };
        }


    }
}
