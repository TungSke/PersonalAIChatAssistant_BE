using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Application.Interfaces
{
    public interface ICharacterEmotionService
    {
        Task<ApiResponse<List<string>>> GetAllDataEmotion();

        Task<ApiResponse<List<CharacterEmotionResponse>>> GetCharacterEmotion(int characterid);
    }
}
