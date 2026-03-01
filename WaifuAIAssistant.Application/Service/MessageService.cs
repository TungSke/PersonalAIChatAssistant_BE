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
        private readonly IRedisCacheService _redisCacheService;

        public MessageService(IUnitOfWork unitOfWork, IJwtService jwtService, IGenerationAIService generationAIService, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _generationAIService = generationAIService;
            _redisCacheService = redisCacheService;
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
            var userId = await _jwtService.GetUserId();

            var conversation = await _unitOfWork.ConversationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.ConversationId);

            if (conversation == null)
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Conversation not found"
                };

            // 1️⃣ Save user message
            var userMessage = new Message
            {
                ConversationId = conversation.Id,
                UserId = userId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MessageRepository.AddAsync(userMessage);
            await _unitOfWork.SaveChangesAsync();

            // 2️⃣ Load character
            var character = await _unitOfWork.ModelRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == conversation.WaifuId);

            if (character == null)
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Conversation not found"
                };

            // Get last 20 messages
            var recentMessages = await _unitOfWork.MessageRepository
                .GetAll()
                .Where(x => x.ConversationId == conversation.Id)
                .OrderByDescending(x => x.Id)
                .Take(20)
                .OrderBy(x => x.Id)
                .ToListAsync();

            //Generate AI reply
            var aiReply = await _generationAIService.GenerateReply(
                conversation,
                character,
                recentMessages,
                request.Content
            );

            // Save AI message
            var aiMessage = new Message
            {
                ConversationId = conversation.Id,
                ModelCharacterId = character.Id,
                Content = aiReply,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MessageRepository.AddAsync(aiMessage);
            await _unitOfWork.SaveChangesAsync();

            //Update summary 
            var messageCount = await _unitOfWork.MessageRepository
                                    .GetAll()
                                    .CountAsync(x => x.ConversationId == conversation.Id);

            if (messageCount % 20 == 0)
            {
                

                var newSummary = await _generationAIService.SummarizeConversation(
                    conversation.Summary,
                    recentMessages
                );

                conversation.Summary = newSummary;

                await _unitOfWork.ConversationRepository.Update(conversation);
                await _unitOfWork.SaveChangesAsync();
            }

            return new ApiResponse<string>
            {
                Success = false,
                Message = aiReply
            };
        }

        public async Task<ApiResponse<string>> DeleteMessage(int messageId)
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