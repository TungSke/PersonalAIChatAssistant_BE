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

            var conversations = await _unitOfWork
                .ConversationRepository
                .GetAll()
                .Where(x => x.UserId == userId)
                .Include(x => x.ModelsCharacter)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();

            return new ApiResponse<List<ConversationResponse>>
            {
                Success = true,
                Message = "Conversations retrieved successfully",
                Data = conversations.Adapt<List<ConversationResponse>>()
            };
        }

        public async Task<ApiResponse<ConversationResponse>> CreateConversation(ConversationRequest request)
        {
            var userId = await _jwtService.GetUserId();

            var modelCharacter = await _unitOfWork.ModelRepository
                .FindAsync(request.ModelCharacterId);

            if (modelCharacter == null)
            {
                throw new KeyNotFoundException("Model character not found");
            }

            var conversationExisted = await _unitOfWork.ConversationRepository
                .GetAll()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ModelCharacterId == request.ModelCharacterId);

            if (conversationExisted != null)
            {
                return new ApiResponse<ConversationResponse>
                {
                    Success = true,
                    Message = "Conversation already exists!",
                    Data = conversationExisted.Adapt<ConversationResponse>()
                };
            }

            var conversation = new Conversation
            {
                UserId = userId,
                ModelCharacterId = request.ModelCharacterId,
                Title = modelCharacter.Name,
                Status = ConversationStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ConversationRepository.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<ConversationResponse>
            {
                Success = true,
                Message = "Created success!",
                Data = conversation.Adapt<ConversationResponse>()
            };
        }

        public async Task<ApiResponse<ConversationResponse>> DeleteConversation(int id)
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
                return new ApiResponse<ConversationResponse>
                {
                    Success = false,
                    Message = e.Message
                };
            }
            return new ApiResponse<ConversationResponse>
            {
                Success = true,
                Message = "Delete success!"
            };
        }


    }
}
