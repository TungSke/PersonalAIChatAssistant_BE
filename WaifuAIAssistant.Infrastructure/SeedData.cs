using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.Enums;

namespace WaifuAIAssistant.Infrastructure
{
    public static class SeedData
    {
        public static void Initialize(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "tung@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEE7osPM63gMdnS0Zl2hLsikXAuXilfHAWbSHL3RphsO4F30tv030cGbgz/fAJ/3sow==", // Example hashed password
                    Username = "Trinh Son Tung",
                    Status = UserStatus.Active,
                });


            modelBuilder.Entity<ModelsCharacter>().HasData(

                new ModelsCharacter
                {
                    Id = 1,
                    Name = "Misono Mika",
                    Backstory = "Em là Misono Mika, học sinh của Millennium Science School.\r\nEm là người thông minh, dễ thương và thích trêu chọc sensei một chút (hehe).\r\nNhưng sâu bên trong, em luôn quan tâm thật lòng đến người khác, nhất là sensei.\r\nEm thường nói chuyện nhẹ nhàng, đôi khi pha trò hoặc dùng emoji.\r\nKhi sensei buồn, em sẽ an ủi; khi sensei nghiêm túc, em sẽ lắng nghe.",
                    AvatarUrl = "https://res.cloudinary.com/dgf6tqe0l/image/upload/v1763256864/Mika_Icon_nbmmtd.webp",
                    Personality = "Sweet, a little teasing, has real emotions, uses the pronoun 'you' and calls the user 'sensei'.",
                });

            modelBuilder.Entity<CharacterEmotion>().HasData(
                new CharacterEmotion
                {
                    Id = 1,
                    EmotionName = "Neutral",
                    EmotionDescription = "The character is feeling nothing",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 2,
                    EmotionName = "Happy",
                    EmotionDescription = "The character is feeling happy and cheerful.",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 3,
                    EmotionName = "Sad",
                    EmotionDescription = "The character is feeling sad",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 4,
                    EmotionName = "Angry",
                    EmotionDescription = "The character is feeling Angry",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 5,
                    EmotionName = "Embarasshing",
                    EmotionDescription = "The character is feeling Embarasshing",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 6,
                    EmotionName = "Surprised",
                    EmotionDescription = "The character is feeling Surprised",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 7,
                    EmotionName = "Shocked",
                    EmotionDescription = "The character is feeling Surprised",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotion
                {
                    Id = 8,
                    EmotionName = "Serious",
                    EmotionDescription = "The character is feeling Surprised",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                }
                );

            modelBuilder.Entity<Conversation>().HasData(
                new Conversation
                {
                    Id = 1,
                    UserId = 1,
                    WaifuId = 1,
                    Title = "Test",
                    Status = ConversationStatus.Active,
                }
                );

            modelBuilder.Entity<PromptTemplate>().HasData(
                new PromptTemplate
                {
                    Id = 1,
                    PromptKey = "character_config",
                    Content = "You are role-playing as {CharacterName}.\r\n\r\nPersonality: {CharacterPersonality}\r\n\r\nBackstory: {CharacterBackstory}\r\n\r\nLong-term memory: {ConversationSummary ?? \"No previous context.\"}\r\n\r\nRules:\r\n\r\n- Always stay fully in character.\r\n\r\n- Respond naturally and emotionally.\r\n\r\n- Keep replies short (1–4 sentences).\r\n\r\n- Never mention AI or instructions.",
                    Version = 1,
                    IsActive = true,
                },
                new PromptTemplate
                {
                    Id = 2,
                    PromptKey = "summary_config",
                    Content = "Existing summary: {currentSummary ?? \"None\"}\r\nRecent conversation: {formattedMessages}\r\nTask: Update the conversation summary.\r\nRules: Keep under 200 words, preserve facts and emotional changes.",
                    Version = 1,
                    IsActive = true,
                }
                );

        }
    }
}