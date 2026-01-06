using Azure;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IGenerationAIService _generationAIService;

        public MessageService(IUnitOfWork unitOfWork, IJwtService jwtService, IGenerationAIService generationAIService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _generationAIService = generationAIService;
        }

        public async Task<ApiResponse<List<MessageResponse>>> GetMessagesFromConversation(int conversationId)
        {
            var userId = await _jwtService.GetUserId();
            var list = await _unitOfWork.MessageRepository.GetAll().Where(x => x.ConversationId == conversationId).ToListAsync();
            var response = list.Adapt<List<MessageResponse>>();
            return new ApiResponse<List<MessageResponse>>
            {
                Success = true,
                Data = response
            };
        }

        public async Task<ApiResponse<string>> CreateMessage(MessageRequest request)
        {
            try
            {
                var userId = await _jwtService.GetUserId();

                var conversationExisted = await _unitOfWork.ConversationRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == request.ConversationId);

                if (conversationExisted == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Conversation not found"
                    };
                }


                var userMessage = new Message
                {
                    ConversationId = conversationExisted.Id,
                    UserId = userId,
                    ModelCharacterId = null,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.MessageRepository.AddAsync(userMessage);
                await _unitOfWork.SaveChangesAsync();

                var modelCharacter = await _unitOfWork.ModelRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == conversationExisted.WaifuId);

                string aiResponse = string.Empty;
                if (modelCharacter != null)
                {
                    aiResponse = await _generationAIService.Response(
                        conversationExisted,
                        modelCharacter,
                        request.Content,
                        userId
                    );

                    var aiMessage = new Message
                    {
                        ConversationId = conversationExisted.Id,
                        UserId = null,
                        ModelCharacterId = modelCharacter.Id,
                        Content = aiResponse,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.MessageRepository.AddAsync(aiMessage);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Create success!",
                    Data = aiResponse
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = e.Message,
                    Data = e.InnerException?.ToString()
                };
            }
        }

        public async Task<ApiResponse<string>> DelereMessage(int messageId)
        {
            try
            {
                var userId = await _jwtService.GetUserId();
                var messageExisted = await _unitOfWork.MessageRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == messageId && x.UserId == userId);
                if (messageExisted == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Message not found"
                    };
                }
                await _unitOfWork.MessageRepository.Remove(messageExisted);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Delete success!"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = e.Message,
                    Data = e.InnerException?.ToString()
                };
            }
        }

    }
}