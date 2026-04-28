using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class GenerationAIService : IGenerationAIService
    {
        // Gemini model is used for generating replies and summarizing conversations.
        private readonly Client _client;
        private readonly string _modelName;
        private readonly IUnitOfWork _unitOfWork;

        public GenerationAIService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _client = new Client(apiKey: configuration["GenerativeAI:AIAPIKey2"]);
            _modelName = configuration["GenerativeAI:ModelName"] ?? "gemini-2.0-flash";
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateReply(
            Conversation conversation,
            ModelsCharacter character,
            List<Message> recentMessages,
            string newUserMessage
        )
        {
            if (conversation == null) throw new ArgumentNullException(nameof(conversation));
            if (character == null) throw new ArgumentNullException(nameof(character));


            var promptemplate = await _unitOfWork.PromptRepository.getPromptValueByName("character_config");
            promptemplate = promptemplate.Replace("{CharacterName}", character.Name)
                                         .Replace("{CharacterPersonality}", character.Personality)
                                         .Replace("{CharacterBackstory}", character.Backstory)
                                         .Replace("{ConversationSummary ?? \"No previous context.\"}", conversation.Summary ?? "No previous context.")
                ;
            var systemInstruction = new Content
            {
                Role = "system",
                Parts = new List<Part> { new Part { Text = promptemplate } }
            };

            var contents = new List<Content>();

            foreach (var msg in recentMessages)
            {
                contents.Add(new Content
                {
                    Role = msg.UserId.HasValue ? "user" : "model",
                    Parts = new List<Part> { new Part { Text = msg.Content } }
                });
            }

            contents.Add(new Content
            {
                Role = "user",
                Parts = new List<Part> { new Part { Text = newUserMessage } }
            });

            var config = new GenerateContentConfig
            {
                SystemInstruction = systemInstruction,
                Temperature = 0.8f,
                TopP = 0.95f,
                MaxOutputTokens = 500
            };

            var response = await _client.Models.GenerateContentAsync(_modelName, contents, config);

            return response.Text;
        }

        public async Task<string> SummarizeConversation(string? currentSummary, List<Message> recentMessages)
        {
            var formattedMessages = string.Join("\n", recentMessages.Select(m =>
                m.UserId.HasValue ? $"User: {m.Content}" : $"Assistant: {m.Content}"));

            var promptemplate = await _unitOfWork.PromptRepository.getPromptValueByName("summary_config");
            promptemplate = promptemplate.Replace("{currentSummary ?? \"None\"}", currentSummary ?? "None")
                                         .Replace("{formattedMessages}", formattedMessages);
                                         

            var contents = new List<Content>
            {
                new Content { Role = "user", Parts = new List<Part> { new Part { Text = promptemplate } } }
            };

            var config = new GenerateContentConfig
            {
                Temperature = 0.3f,
                MaxOutputTokens = 300
            };

            var response = await _client.Models.GenerateContentAsync(_modelName, contents, config);

            return response.Text;
        }
    }
}