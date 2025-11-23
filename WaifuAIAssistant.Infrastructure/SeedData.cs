using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.Enums;

namespace WaifuAIAssistant.Infrastructure
{
    public static class SeedData
    {
        public static async Task Initialize(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    Id = 1,
                    Email = "tung@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEE7osPM63gMdnS0Zl2hLsikXAuXilfHAWbSHL3RphsO4F30tv030cGbgz/fAJ/3sow==", // Example hashed password
                    Username = "Trinh Son Tung",
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 1),
                });


            modelBuilder.Entity<ModelsCharacter>().HasData(

                new ModelsCharacter
                {
                    Id = 1,
                    Name = "Misono Mika",
                    Backstory = "Em là Misono Mika, học sinh của Millennium Science School.\r\nEm là người thông minh, dễ thương và thích trêu chọc sensei một chút (hehe).\r\nNhưng sâu bên trong, em luôn quan tâm thật lòng đến người khác, nhất là sensei.\r\nEm thường nói chuyện nhẹ nhàng, đôi khi pha trò hoặc dùng emoji.\r\nKhi sensei buồn, em sẽ an ủi; khi sensei nghiêm túc, em sẽ lắng nghe.",
                    AvatarUrl = "https://res.cloudinary.com/dgf6tqe0l/image/upload/v1763256864/Mika_Icon_nbmmtd.webp",
                    Personality = "Sweet, a little teasing, has real emotions, uses the pronoun 'you' and calls the user 'sensei'.",
                    CreatedAt = new DateTime(2024, 1, 1),
                    UpdatedAt = new DateTime(2024, 1, 1)
                });

            modelBuilder.Entity<CharacterEmotions>().HasData(
                new CharacterEmotions
                {
                    Id = 1,
                    EmotionName = "Neutral",
                    EmotionDescription = "The character is feeling nothing",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 2,
                    EmotionName = "Happy",
                    EmotionDescription = "The character is feeling happy and cheerful.",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 3,
                    EmotionName = "Sad",
                    EmotionDescription = "The character is feeling sad",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 4,
                    EmotionName = "Angry",
                    EmotionDescription = "The character is feeling Angry",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 5,
                    EmotionName = "Embarasshing",
                    EmotionDescription = "The character is feeling Embarasshing",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 6,
                    EmotionName = "Surprised",
                    EmotionDescription = "The character is feeling Surprised",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 7,
                    EmotionName = "Shocked",
                    EmotionDescription = "The character is feeling Surprised",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
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
                    CreatedAt = new DateTime(2024, 1, 1),
                    UpdatedAt = new DateTime(2024, 1, 1),
                    Status = ConversationStatus.Active,
                }
                );
        }
    }
}