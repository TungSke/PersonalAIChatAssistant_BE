using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Application.Interfaces
{
    public interface ICharacterEmotionService
    {
        Task<ApiResponse<List<string>>> GetAllDataEmotion();

        Task<ApiResponse<List<CharacterEmotionResponse>>> GetCharacterEmotion(int characterid);
    }
}
