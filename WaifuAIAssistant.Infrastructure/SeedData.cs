using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaifuAIAssistant.Domain.Entities;

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
                    PasswordHash = "$2a$11$eW5z1Z3b1Q8f5k5j5k5j5uO5z1Z3b1Q8f5k5j5k5j5uO5z1Z3b1Q8", // Example hashed password
                    Username = "Trinh Son Tung",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                });


            modelBuilder.Entity<ModelsCharacter>().HasData(

                new ModelsCharacter
                {
                    Id = 1,
                    Name = "Misono Mika",
                    Backstory = "This is a default character for the application.",
                    AvatarUrl = "https://example.com/default-character.png",
                    Personality = "Friendly and helpful",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

            modelBuilder.Entity<CharacterEmotions>().HasData(
                new CharacterEmotions
                {
                    Id = 1,
                    EmotionName = "Happy",
                    EmotionDescription = "The character is feeling happy and cheerful.",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 2,
                    EmotionName = "Sad",
                    EmotionDescription = "The character is feeling sad",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 3,
                    EmotionName = "Angry",
                    EmotionDescription = "The character is feeling Angry",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 4,
                    EmotionName = "Embarasshing",
                    EmotionDescription = "The character is feeling Embarasshing",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                },
                new CharacterEmotions
                {
                    Id = 5,
                    EmotionName = "Hatred",
                    EmotionDescription = "The character is feeling Hatred",
                    EmotionIconUrl = "",
                    CharacterId = 1,
                }
                );
        }
    }
}