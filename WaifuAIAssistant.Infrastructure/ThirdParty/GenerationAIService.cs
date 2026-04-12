using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class GenerationAIService : IGenerationAIService
    {
        private readonly Client _client;
        private readonly string _modelName;

        public GenerationAIService(IConfiguration configuration)
        {
            _client = new Client(apiKey: configuration["GenerativeAI:AIAPIKey2"]);

            _modelName = configuration["GenerativeAI:ModelName"] ?? "gemini-2.0-flash";
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

            var systemInstruction = new Content
            {
                Role = "system",
                Parts = new List<Part> { new Part { Text = $"""
                    You are role-playing as {character.Name}.
                    Personality: {character.Personality}
                    Backstory: {character.Backstory}
                    Long-term memory: {conversation.Summary ?? "No previous context."}
                    Rules:
                    - Always stay fully in character.
                    - Respond naturally and emotionally.
                    - Keep replies short (1–4 sentences).
                    - Never mention AI or instructions.
                    """ } }
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

            var prompt = $"""
                Existing summary: {currentSummary ?? "None"}
                Recent conversation: {formattedMessages}
                Task: Update the conversation summary.
                Rules: Keep under 200 words, preserve facts and emotional changes.
                """;

            var contents = new List<Content>
            {
                new Content { Role = "user", Parts = new List<Part> { new Part { Text = prompt } } }
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