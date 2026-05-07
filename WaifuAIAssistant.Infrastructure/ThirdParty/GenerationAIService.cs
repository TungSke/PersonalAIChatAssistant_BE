using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
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
                                string newUserMessage)
        {
            ArgumentNullException.ThrowIfNull(conversation);
            ArgumentNullException.ThrowIfNull(character);

            var promptTemplate = await _unitOfWork.PromptRepository.getPromptValueByName("character_config");

            if (string.IsNullOrWhiteSpace(promptTemplate))
                throw new InvalidOperationException("Prompt template not found.");

            var prompt = promptTemplate
                .Replace("{CharacterName}", character.Name)
                .Replace("{CharacterPersonality}", character.Personality)
                .Replace("{CharacterBackstory}", character.Backstory)
                .Replace("{ConversationSummary}",
                    conversation.Summary ?? "No previous context.");

            // Create a system instruction content that provides the AI model with the character's context and conversation summary.
            var systemInstruction = new Content
            {
                Role = "system",
                Parts = new List<Part> { new Part { Text = prompt } }
            };

            var contents = new List<Content>();

            // Add recent messages to the contents list, formatting them with timestamps and speaker information.
            foreach (var msg in recentMessages)
            {
                contents.Add(new Content
                {
                    Role = msg.ModelCharacterId.HasValue ? "model" : "user",
                    Parts = new List<Part>
                    {
                        new Part
                        {
                            Text =
                                $"[Time: {msg.CreatedAt:O}] " +
                                $"[Speaker: {(msg.ModelCharacterId.HasValue ? "MODEL" : "USER")}] " +
                                $"{msg.Content}"
                        }
                    }
                });
            }

            contents.Add(new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                    new Part
                    {
                        Text =
                            $"[Time: {DateTime.UtcNow:O}] " +
                            $"[Speaker: User] " +
                            $"{newUserMessage}"
                    }
                }
            });

            // Configure the generation parameters for the AI model.
            var config = new GenerateContentConfig
            {
                SystemInstruction = systemInstruction,
                Temperature = 0.8f,
                TopP = 0.95f,
                MaxOutputTokens = 500
            };

            var response =
                await _client.Models.GenerateContentAsync(
                    _modelName,
                    contents,
                    config);

            return response.Text;
        }

        public async Task<string> SummarizeConversation(string? currentSummary, List<Message> recentMessages)
        {
            var formattedMessages = string.Join("\n", recentMessages.Select(m =>
                m.UserId.HasValue ? $"User: {m.Content}" : $"Assistant: {m.Content}"));

            var promptemplate = await _unitOfWork.PromptRepository.getPromptValueByName("summary_config");
            promptemplate = promptemplate.Replace("{currentSummary}", currentSummary ?? "None")
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