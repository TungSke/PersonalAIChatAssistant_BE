using Mapster;
using Microsoft.EntityFrameworkCore;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Application.Interfaces;
using PersonalAIAssistant.Domain;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.ThirdPartyInterface;

namespace PersonalAIAssistant.Application.Service
{
    public class ModelsCharacterService : IModelsCharacterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;    
        private readonly IRedisCacheService _redisCacheService;
        public ModelsCharacterService(IUnitOfWork unitOfWork, IJwtService jwtService, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task<ApiResponse<IEnumerable<ModelCharacterResponse>>> GetAllAsync(
                                                                                        int pageIndex,
                                                                                        int pageSize,
                                                                                        string? search)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var userId = await _jwtService.GetUserId();
            search = search?.Trim();

            var chattedCharacterIdsQuery = _unitOfWork.ConversationRepository
                .GetAll()
                .Where(c => c.UserId == userId)
                .Select(c => c.ModelCharacterId);

            var query = _unitOfWork.ModelRepository
                .GetAll()
                .Where(m => !chattedCharacterIdsQuery.Contains(m.Id));

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => EF.Functions.Like(m.Name, $"%{search}%"));
            }

            var characters = await query
                .OrderBy(m => m.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = characters.Adapt<List<ModelCharacterResponse>>();

            return new ApiResponse<IEnumerable<ModelCharacterResponse>>
            {
                Success = true,
                Message = characters.Any()
                    ? "Model characters retrieved successfully."
                    : "No model characters found.",
                Data = response
            };
        }


    }
}
