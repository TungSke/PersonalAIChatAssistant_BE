using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    // Responsible ONLY for AI prompt & response
    public class GenerationAIService : IGenerationAIService
    {
        private readonly GenerativeModel _model;

        public GenerationAIService(IConfiguration configuration)
        {
            var google = new GoogleAi(configuration["GenerativeAI:AIAPIKey"]);

            _model = google.CreateGenerativeModel(
                modelName: "models/gemini-3-flash-preview",
                config: new GenerationConfig
                {
                    Temperature = 0.8f,
                    TopP = 0.95f,
                    MaxOutputTokens = 500
                }
            );
        }

        public async Task<string> GenerateReply(
            Conversation conversation,
            ModelsCharacter character,
            List<Message> recentMessages,
            string newUserMessage
        )
        {
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            if (character == null)
                throw new ArgumentNullException(nameof(character));

            _model.SystemInstruction = $"""
            You are role-playing as {character.Name}.

            Personality:
            {character.Personality}

            Backstory:
            {character.Backstory}

            Long-term memory:
            {conversation.Summary ?? "No previous context."}

            Rules:
            - Always stay fully in character as the role — never break character or step out of it.
            - Respond naturally and emotionally, just like you're really texting someone you care about.
            - Keep replies short and casual (1–4 sentences, roughly 100–250 tokens), unless the user specifically asks for a longer, more detailed response or story.
            - Never mention AI, prompts, rules, system instructions, guidelines, or anything outside of the character's role and perspective.
            """;

            var contents = new List<Content>();

            // 👉 context ngắn hạn (10–20 messages)
            foreach (var msg in recentMessages)
            {
                contents.Add(new Content
                {
                    Role = msg.UserId.HasValue ? "user" : "model",
                    Parts = { new Part { Text = msg.Content } }
                });
            }

            // 👉 message mới nhất của user
            contents.Add(new Content
            {
                Role = "user",
                Parts = { new Part { Text = newUserMessage } }
            });

            var response = await _model.GenerateContentAsync(
                new GenerateContentRequest
                {
                    Contents = contents
                });

            return response.Text();
        }

        //recent message is 20 last message, not include system instruction
        public async Task<string> SummarizeConversation(
    string? currentSummary,
    List<Message> recentMessages
)
        {
            var formattedMessages = recentMessages
                .Select(m =>
                    m.UserId.HasValue
                        ? $"User: {m.Content}"
                        : $"Assistant: {m.Content}"
                );

            var prompt = $"""
    Existing summary:
    {currentSummary ?? "None"}

    Recent conversation:
    {string.Join("\n", formattedMessages)}

    Task:
    Update the conversation summary.

    Rules:
    - Keep under 200 words
    - Preserve important facts
    - Remember user preferences
    - Track relationship & emotional changes
    - Do NOT repeat full dialogue
    """;

            var response = await _model.GenerateContentAsync(
                new GenerateContentRequest
                {
                    Contents =
                    {
                new Content
                {
                    Role = "user",
                    Parts = { new Part { Text = prompt } }
                }
                    },
                    GenerationConfig = new GenerationConfig
                    {
                        Temperature = 0.3f,
                        MaxOutputTokens = 300
                    }
                });

            return response.Text();
        }
    }
}