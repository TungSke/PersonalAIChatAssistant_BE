using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class GenerationAIService : IGenerationAIService
    {
        private readonly IConfiguration _configuration;
        private readonly GenerativeModel _model;
        private readonly IUnitOfWork _unitOfWork;

        public GenerationAIService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;

            var google = new GoogleAi(_configuration["GenerativeAI:AIAPIKey"]);
            _model = google.CreateGenerativeModel("models/gemini-2.0-flash");
        }

        public async Task<string> Response(int conversationId, ModelsCharacter modelsCharacter, string newUserMessage, int userId)
        {
            var conversation = await _unitOfWork.ConversationRepository
                .GetAll()
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.Id == conversationId);

            if (conversation == null)
                throw new Exception("Conversation not found");

            var userMsg = new Message
            {
                ConversationId = conversation.Id,
                UserId = userId,
                Content = newUserMessage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MessageRepository.AddAsync(userMsg);
            await _unitOfWork.SaveChangesAsync();

            // Build lịch sử cho AI
            var contents = new List<Content>();

            // Gắn backstory làm system instruction
            _model.SystemInstruction = modelsCharacter.Backstory;

            foreach (var msg in conversation.Messages.OrderBy(m => m.CreatedAt))
            {
                if (!string.IsNullOrEmpty(msg.Content))
                {
                    // user hoặc model
                    var role = msg.UserId.HasValue ? "user" : "model";
                    contents.Add(new Content
                    {
                        Role = role,
                        Parts = new List<Part> { new Part { Text = msg.Content } }
                    });
                }
            }

            // Thêm message mới nhất của user
            contents.Add(new Content
            {
                Role = "user",
                Parts = new List<Part> { new Part { Text = newUserMessage } }
            });

            // Gọi API AI
            var response = await _model.GenerateContentAsync(new GenerateContentRequest
            {
                Contents = contents
            });

            var aiReply = response.Text();

            // Lưu message AI
            //var aiMsg = new Message
            //{
            //    ConversationId = conversation.Id,
            //    ModelCharacterId = modelsCharacter.Id,
            //    Content = aiReply,
            //    CreatedAt = DateTime.UtcNow,
            //    UpdatedAt = DateTime.UtcNow
            //};

            //await _unitOfWork.MessageRepository.AddAsync(aiMsg);
            //await _unitOfWork.SaveChangesAsync();

            return aiReply;
        }
    }

}