using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
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

        private readonly int _defaultLimit = 50;

        public async Task<ApiResponse<MessageListResponse>> GetMessagesFromConversation(
            int conversationId,
            int limit,
            long? beforeMessageId = null)
        {

            var userId = await _jwtService.GetUserId();

            if (limit <= 0)
            {
                limit = _defaultLimit;
            }

            if (limit > _defaultLimit)
            {
                return new ApiResponse<MessageListResponse>
                {
                    Success = false,
                    Message = $"Limit cannot exceed {_defaultLimit}"
                };
            }

            // Validate conversation exists and belongs to current user
            var conversation = await _unitOfWork.ConversationRepository
                .GetAll()
                .Include(x => x.ModelsCharacter)
                .FirstOrDefaultAsync(x => x.Id == conversationId && x.UserId == userId);

            if (conversation == null)
            {
                throw new KeyNotFoundException("Conversation not found!");
            }

            if (beforeMessageId == null)
            {
                var cacheKey = $"chat:{conversationId}:latest:{limit}:user:{userId}";

                var cachedResponse = await _redisCacheService
                    .GetAsync<MessageListResponse>(cacheKey);

                if (cachedResponse != null)
                {
                    return new ApiResponse<MessageListResponse>
                    {
                        Success = true,
                        Message = "Data from cache",
                        Data = cachedResponse
                    };
                }
            }

            var query = _unitOfWork.MessageRepository
                .GetAll()
                .Where(x => x.ConversationId == conversationId);

            if (beforeMessageId != null)
            {
                query = query.Where(x => x.Id < beforeMessageId);
            }

            var messages = await query
                .OrderByDescending(x => x.Id)
                .Take(limit)
                .ToListAsync();

            messages = messages
                .Take(limit)
                .OrderBy(x => x.Id)
                .ToList();

            var response = new MessageListResponse
            {
                ModelId = conversation.ModelCharacterId.ToString(),
                ModelName = conversation.ModelsCharacter?.Name, 
                ModelAvatarUrl = conversation.ModelsCharacter?.AvatarUrl,
                FirstMessageId = messages.LastOrDefault()?.Id,

                Messages = messages.Adapt<List<MessageResponse>>()
            };


            if (beforeMessageId == null)
            {
                var cacheKey = $"chat:{conversationId}:latest:{limit}:user:{userId}";

                await _redisCacheService.SetAsync(
                    cacheKey,
                    response,
                    TimeSpan.FromSeconds(30));
            }

            return new ApiResponse<MessageListResponse>
            {
                Success = true,
                Message = messages.Any()
                    ? "Messages retrieved successfully"
                    : "No messages found in this conversation",
                Data = response
            };
        }


        public async Task<ApiResponse<MessageResponse>> CreateMessage(MessageRequest request)
        {
            try
            {
                var userId = await _jwtService.GetUserId();

                // Validate conversation exists and belongs to current user
                var conversation = await _unitOfWork.ConversationRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == request.ConversationId && x.UserId == userId);

                if (conversation == null)
                {
                    throw new KeyNotFoundException("Conversation not found or does not belong to user");
                }

                // Validate character exists before calling AI
                var character = await _unitOfWork.ModelRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == conversation.ModelCharacterId);

                if (character == null)
                {
                    throw new KeyNotFoundException("Character not found for this conversation");
                }

                // Get recent messages for context before calling AI
                var recentMessages = await _unitOfWork.MessageRepository
                    .GetAll()
                    .Where(x => x.ConversationId == conversation.Id)
                    .OrderByDescending(x => x.Id)
                    .Take(20)
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                // Call AI first before saving anything to DB
                // If AI fails, nothing gets saved and we return error immediately
                var aiReply = await _generationAIService.GenerateReply(
                    conversation,
                    character,
                    recentMessages,
                    request.Content
                );

                // AI succeeded — now save both messages in a single transaction
                // If DB fails here, both user message and AI reply are rolled back together
                Message aiMessage = null;

                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // Save user message
                    var userMessage = new Message
                    {
                        ConversationId = conversation.Id,
                        UserId = userId,
                        Content = request.Content,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.MessageRepository.AddAsync(userMessage);

                    // Save AI reply
                    aiMessage = new Message
                    {
                        ConversationId = conversation.Id,
                        ModelCharacterId = character.Id,
                        Content = aiReply,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.MessageRepository.AddAsync(aiMessage);

                    conversation.UpdatedAt = DateTime.UtcNow;

                    // Every 20 messages, summarize the conversation to keep context window short
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
                    }
                });

                return new ApiResponse<MessageResponse>
                {
                    Success = true,
                    Message = "Send message successfully!",
                    Data = aiMessage.Adapt<MessageResponse>()
                };
            }
            catch (Exception ex)
            {
                // No manual rollback needed — ExecuteInTransactionAsync handles it internally
                return new ApiResponse<MessageResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
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
                    throw new KeyNotFoundException("Message not found or does not belong to user");
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

        public async Task<ApiResponse<string>> ExportConversationtoPDF(int conversationId)
        {
            try
            {
                var userId = await _jwtService.GetUserId();
                var conversationExisted = await _unitOfWork.ConversationRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == conversationId && x.UserId == userId);
                if (conversationExisted == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Conversation not found"
                    };
                }
                var messages = await _unitOfWork.MessageRepository
                    .GetAll()
                    .Where(x => x.ConversationId == conversationId)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();



                return new ApiResponse<string>
                {
                    Success = true,
                    Data = "exportContent"
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