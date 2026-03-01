using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.Enums;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Application.Service
{
    public class CharacterEmotionService : ICharacterEmotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;
        public CharacterEmotionService(IUnitOfWork unitOfWork, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
        }

        public async Task<ApiResponse<List<string>>> GetAllDataEmotion()
        {
            var list = Enum.GetNames(typeof(EmotionName));
            return new ApiResponse<List<string>>
            {
                Success = true,
                Data = list.ToList()
            };
        }

        public async Task<ApiResponse<List<CharacterEmotionResponse>>> GetCharacterEmotion(int characterid)
        {
            var cacheKey = $"character_emotions_{characterid}";
            var cacheCharacterEmotions = await _redisCacheService.GetAsync<List<CharacterEmotionResponse>>(cacheKey);
            if (cacheCharacterEmotions != null)
            {
                return new ApiResponse<List<CharacterEmotionResponse>>
                {
                    Success = true,
                    Data = cacheCharacterEmotions
                };
            }

            var list = await _unitOfWork.CharacterEmotionsRepository.GetAll().Where(x => x.CharacterId == characterid).ToListAsync();

            if(list == null || list.Count == 0)
            {
                throw new KeyNotFoundException("No emotions found for the specified character.");
            }

            var response = list.Adapt<List<CharacterEmotionResponse>>();

            return new ApiResponse<List<CharacterEmotionResponse>>
            {
                Success = true,
                Data = response
            };
        }
    }
}
