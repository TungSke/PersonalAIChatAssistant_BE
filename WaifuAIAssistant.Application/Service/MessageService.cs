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
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        public MessageService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<List<MessageResponse>>> GetMessagesFromConversation(int conversationId)
        {
            var userId = await _jwtService.GetUserId();
            var list = await _unitOfWork.MessageRepository.GetAll().Where(x => x.ConversationId == conversationId && x.UserId == userId).ToListAsync();
            var response = list.Adapt<List<MessageResponse>>();
            return new ApiResponse<List<MessageResponse>>
            {
                Success = true,
                Data = response
            };
        }

        public async Task<ApiResponse<MessageRequest>> CreateMessage(MessageRequest request)
        {
            try
            {
                var userId = await _jwtService.GetUserId();
                var conversationExisted = await _unitOfWork.ConversationRepository.GetAll().FirstOrDefaultAsync(x => x.Id == request.ConversationId);
                if(conversationExisted == null)
                {
                    return new ApiResponse<MessageRequest>
                    {
                        Success = false,
                        Message = "Not found conversation",
                    };
                }

                var messageCreate = request.Adapt<Message>();
                messageCreate.UserId = userId;
                await _unitOfWork.MessageRepository.AddAsync(messageCreate);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ApiResponse<MessageRequest>
                {
                    Success = false,
                    Message = e.Message
                };
            }
            return new ApiResponse<MessageRequest>
            {
                Success = true,
                Message = "Create success!",
                Data = request
            };
        }
    }
}