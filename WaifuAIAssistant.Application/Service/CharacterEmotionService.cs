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

        public async Task<ApiResponse<List<CharacterEmotions>>> GetCharacterEmotion(int characterid)
        {
            var list = await _unitOfWork.CharacterEmotionsRepository.GetAll().Where(x => x.CharacterId == characterid).ToListAsync();

            return new ApiResponse<List<CharacterEmotions>>
            {
                Success = true,
                Data = list
            };
        }
    }
}
