using Mapster;
using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.Enums;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Application.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IRedisCacheService _redisCacheService;
        public ConversationService(IUnitOfWork unitOfWork, IJwtService jwtService, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _redisCacheService = redisCacheService;
        }

        public async Task<ApiResponse<List<ConversationResponse>>> GetConversationAsync()
        {
            var userId = await _jwtService.GetUserId();

            if(userId != 0)
            {
                var cacheKey = $"conversations_user_{userId}";
                var cachedConversations = await _redisCacheService.GetAsync<List<ConversationResponse>>(cacheKey);
                if (cachedConversations != null)
                {
                    return new ApiResponse<List<ConversationResponse>>
                    {
                        Success = true,
                        Message = "Conversations retrieved successfully from cache",
                        Data = cachedConversations,
                    };
                }
            }

            var conversations = await _unitOfWork.ConversationRepository.GetAll()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var response = conversations.Adapt<List<ConversationResponse>>();
            if(userId != 0)
            {
                var cacheKey = $"conversations_user_{userId}";
                await _redisCacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30));
            }

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
                var modelCharacter = await _unitOfWork.ModelRepository.FindAsync(request.WaifuId);

                var conversationExisted = await _unitOfWork.ConversationRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == userId && x.ModelCharacterId == request.WaifuId);
                if (conversationExisted != null)
                {
                    throw new Exception("Conversation already exists!");
                }

                var createCon = request.Adapt<Conversation>();
                createCon.UserId = userId;
                createCon.Title = string.IsNullOrEmpty(request.Title) ? modelCharacter.Name : request.Title;
                createCon.Status = ConversationStatus.Active;

                await _unitOfWork.ConversationRepository.AddAsync(createCon);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<ConversationRequest>
                {
                    Success = true,
                    Message = "Created success!",
                    Data = request
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create conversation: {e.Message}");
            }
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
