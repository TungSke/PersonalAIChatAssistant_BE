using Mapster;
using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
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

        public async Task<ApiResponse<List<ConversationResponse>>> GetConversationAsync()
        {
            var userId = await _jwtService.GetUserId();

            var conversations = await _unitOfWork.ConversationRepository.GetAll()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var response = conversations.Adapt<List<ConversationResponse>>();

            return new ApiResponse<List<ConversationResponse>>
            {
                Data = response,
                Message = "Conversations retrieved successfully",
                Success = true
            };
        }

        public async Task<ApiResponse<ConversationRequest>> CreateConversation(ConversationRequest request)
        {
            try
            {
                var userId = await _jwtService.GetUserId();
                var createCon = request.Adapt<Conversation>();
                createCon.UserId = userId;
                await _unitOfWork.ConversationRepository.AddAsync(createCon);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ApiResponse<ConversationRequest>
                {
                    Success = false,
                    Message = e.Message
                };
            }
            return new ApiResponse<ConversationRequest>
            {
                Success = true,
                Message = "Create success!",
                Data = request
            };
        }

        public async Task<ApiResponse<ConversationRequest>> DeleteConversation(int id)
        {
            try
            {
                var userId = await _jwtService.GetUserId();
                var conversationExisted = await _unitOfWork.ConversationRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
                if (conversationExisted != null)
                {
                    await _unitOfWork.ConversationRepository.Remove(conversationExisted);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<ConversationRequest>
                {
                    Success = false,
                    Message = e.Message
                };
            }
            return new ApiResponse<ConversationRequest>
            {
                Success = true,
                Message = "Create success!"
            };
        }


    }
}
