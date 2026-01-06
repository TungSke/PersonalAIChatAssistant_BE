using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    // manage AI Prompt and response from Generative AI 
    // using Google Gemini API
    // https://learn.microsoft.com/en-us/dotnet/api/generativeai.googleai?view=generativeai-dotnet-preview
    // The performance of this service can be improved by caching the conversation context
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

        public async Task<string> Response(Conversation conversation, ModelsCharacter modelsCharacter, string newUserMessage, int userId)
        {

            if (conversation == null)
                throw new Exception("Conversation not found");

            _model.SystemInstruction = $@"
                You are now acting as the character: {modelsCharacter.Name}.

                Personality:
                {modelsCharacter.Personality}

                Backstory:
                {modelsCharacter.Backstory}

                Behavioral Rules:
                - Always stay in character.
                - Use natural and expressive tone.
                - If the personality implies affection, express it subtly (not robotic).
                - Reply concisely unless asked to explain in detail.
                ";


            var contents = new List<Content>();

            foreach (var msg in conversation.Messages.OrderBy(m => m.CreatedAt))
            {
                if (!string.IsNullOrEmpty(msg.Content))
                {
                    var role = msg.UserId.HasValue ? "user" : "model";
                    contents.Add(new Content
                    {
                        Role = role,
                        Parts = new List<Part> { new Part { Text = msg.Content } }
                    });
                }
            }

            contents.Add(new Content
            {
                Role = "user",
                Parts = new List<Part> { new Part { Text = newUserMessage } }
            });

            // Gọi API AI
            var response = await _model.GenerateContentAsync(new GenerateContentRequest
            {
                Contents = contents,
            });

            var aiReply = response.Text();

            return aiReply;
        }
    }

}