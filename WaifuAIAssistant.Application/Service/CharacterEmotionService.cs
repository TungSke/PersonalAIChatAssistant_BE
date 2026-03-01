using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.Enums;

namespace WaifuAIAssistant.Application.Service
{
    public class CharacterEmotionService : ICharacterEmotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CharacterEmotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task<ApiResponse<List<CharacterEmotion>>> GetCharacterEmotion(int characterid)
        {
            var list = await _unitOfWork.CharacterEmotionsRepository.GetAll().Where(x => x.CharacterId == characterid).ToListAsync();

            if(list == null || list.Count == 0)
            {
                throw new KeyNotFoundException("No emotions found for the specified character.");
            }

            var response = list.Adapt<List<CharacterEmotion>>();

            return new ApiResponse<List<CharacterEmotion>>
            {
                Success = true,
                Data = response
            };
        }
    }
}
