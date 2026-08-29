using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Enums;

namespace PersonalAIAssistant.Infrastructure
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
        }
    }
}