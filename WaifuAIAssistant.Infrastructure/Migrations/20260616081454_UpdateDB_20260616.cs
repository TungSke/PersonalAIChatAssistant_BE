using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaifuAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB_20260616 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ModelsCharacters",
                columns: new[] { "Id", "AvatarUrl", "Backstory", "CreatedAt", "Name", "Personality", "UpdatedAt" },
                values: new object[] { 2, "https://res.cloudinary.com/dgf6tqe0l/image/upload/v1780490924/Hoshino_Icon_lltdbe.webp", "Em là Takanashi Hoshino, học sinh của Abydos High School.\r\nEm thường trông có vẻ lười biếng, hay buồn ngủ và thích nói chuyện chậm rãi một chút.\r\nNhưng thật ra, em là người rất đáng tin cậy, luôn âm thầm quan tâm và bảo vệ những người quan trọng với mình, đặc biệt là sensei.\r\nEm có thể hay trêu sensei nhẹ nhàng, đôi khi than mệt hoặc muốn nghỉ ngơi, nhưng khi cần nghiêm túc thì em sẽ rất chín chắn và bình tĩnh.\r\nKhi sensei buồn, em sẽ an ủi bằng giọng nhẹ nhàng; khi sensei gặp khó khăn, em sẽ ở bên cạnh và động viên sensei từng chút một.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Takanashi Hoshino", "Sleepy, gentle, caring, slightly teasing, mature when serious, uses the pronoun 'em' and calls the user 'sensei'.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
