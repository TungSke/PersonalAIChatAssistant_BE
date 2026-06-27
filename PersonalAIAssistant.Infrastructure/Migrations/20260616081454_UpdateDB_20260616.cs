using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalAIAssistant.Infrastructure.Migrations
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
                values: new object[] { 2, "https://res.cloudinary.com/dgf6tqe0l/image/upload/v1780490924/Hoshino_Icon_lltdbe.webp", "Em là Takanashi Hoshino, h?c sinh c?a Abydos High School.\r\nEm thu?ng trông có v? lu?i bi?ng, hay bu?n ng? và thích nói chuy?n ch?m rãi m?t chút.\r\nNhung th?t ra, em là ngu?i r?t dáng tin c?y, luôn âm th?m quan tâm và b?o v? nh?ng ngu?i quan tr?ng v?i mình, d?c bi?t là sensei.\r\nEm có th? hay trêu sensei nh? nhàng, dôi khi than m?t ho?c mu?n ngh? ngoi, nhung khi c?n nghiêm túc thì em s? r?t chín ch?n và bình tinh.\r\nKhi sensei bu?n, em s? an ?i b?ng gi?ng nh? nhàng; khi sensei g?p khó khan, em s? ? bên c?nh và d?ng viên sensei t?ng chút m?t.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Takanashi Hoshino", "Sleepy, gentle, caring, slightly teasing, mature when serious, uses the pronoun 'em' and calls the user 'sensei'.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
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
