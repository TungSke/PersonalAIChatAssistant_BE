using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Application.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        public ConversationService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<List<Conversation>>> GetConversationAsync(string jwttoken)
        {
            var userId = _jwtService.GetUserIdFromJwt(jwttoken);

            var conversations = await _unitOfWork.ConversationRepository.GetAll()
                .Where(x => x.UserId == userId)
                .ToListAsync();
            
            return new ApiResponse<List<Conversation>> { 
                Data = conversations,
                Message = "Conversations retrieved successfully",
                Success = true
            };
        }
    }
}
