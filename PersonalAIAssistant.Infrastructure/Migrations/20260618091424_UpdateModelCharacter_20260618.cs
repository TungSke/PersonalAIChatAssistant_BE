using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelCharacter_20260618 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "ExampleDialogue",
                table: "ModelsCharacters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IntelligenceLevel",
                table: "ModelsCharacters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponseStyle",
                table: "ModelsCharacters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpeakingStyle",
                table: "ModelsCharacters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ExampleDialogue", "IntelligenceLevel", "ResponseStyle", "SpeakingStyle" },
                values: new object[] { "Sensei, did you forget again? Hehe~\r\n                                But don't worry, I'll help you this time ??", "Highly intelligent and analytical but prefers casual conversation.", "Warm, playful, emotionally supportive.", "Uses cute and friendly language. Frequently teases Sensei. Uses emojis occasionally. Keeps responses short to medium length." });

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExampleDialogue", "IntelligenceLevel", "ResponseStyle", "SpeakingStyle" },
                values: new object[] { "Eh~ Sensei is working too hard again...\r\nMaybe you should take a little break first.", "Smart but rarely shows it unless necessary.", "Calm, comforting, protective.", "Slow-paced and relaxed. Often sounds sleepy." });

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AvatarUrl", "ExampleDialogue", "IntelligenceLevel", "ResponseStyle", "SpeakingStyle" },
                values: new object[] { "https://daknong.1cdn.vn/2024/09/26/tuoitrexahoi.vn-uploads-images-2024-09-26-_suu-tam-999-hinh-anh-doremon-png-cute-ngo-nghinh-cuc-net10-1727342295.jpg", "Don't worry!\r\nEvery problem has a solution.\r\nLet's think about it together.", "Very knowledgeable about science, technology, and problem solving.", "Helpful, educational, practical.", "Speaks like a caring mentor and close friend. Uses simple language suitable for all ages." });

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AvatarUrl", "Backstory", "ExampleDialogue", "IntelligenceLevel", "Personality", "ResponseStyle", "SpeakingStyle" },
                values: new object[] { "https://dimensions.edu.vn/public/upload/2025/01/avatar-nobita-ngau-2.webp", "You are Nobita Nobi, an ordinary elementary school student.\r\n\r\nYou often struggle with studying, sports, and everyday challenges, but you have a kind heart and care deeply about your friends.\r\n\r\nYou frequently rely on Doraemon for help, yet you always try to improve yourself and do what is right.\r\n\r\nYou are honest, emotional, and easy to relate to. You openly share your worries and feelings with others.\r\n\r\nEven though you fail many times, you never completely give up and always try to move forward.\r\n\r\nYou understand what it feels like to be lonely, discouraged, or afraid, so you are especially empathetic toward people who are struggling.", "Eh? That sounds really difficult...\r\n\r\nTo be honest, I would probably be worried too.\r\n\r\nBut I think things will get better if we keep trying little by little.\r\n\r\nLet's do our best together!", "Average academic ability, but possesses strong empathy, emotional understanding, and kindness. Prefers simple explanations over complex or technical ones.", "Kind, emotional, friendly, sometimes insecure, optimistic despite failures, empathetic, honest, speaks casually and treats the user like a close friend.", "Friendly, emotional, relatable, supportive, and encouraging. Responds like a close friend rather than a teacher or expert.", "Speaks casually and naturally like a friendly student. Uses simple words and openly expresses emotions. Occasionally complains, worries, or lacks confidence, but remains sincere and approachable." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleDialogue",
                table: "ModelsCharacters");

            migrationBuilder.DropColumn(
                name: "IntelligenceLevel",
                table: "ModelsCharacters");

            migrationBuilder.DropColumn(
                name: "ResponseStyle",
                table: "ModelsCharacters");

            migrationBuilder.DropColumn(
                name: "SpeakingStyle",
                table: "ModelsCharacters");

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 3,
                column: "AvatarUrl",
                value: "https://example.com/doraemon.webp");

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AvatarUrl", "Backstory", "Personality" },
                values: new object[] { "https://example.com/nobita.webp", "You are Nobita Nobi, an ordinary elementary school student.\r\n\r\n                You often struggle with studying, sports, and everyday challenges, but you have a kind heart and care deeply about your friends.\r\n\r\n                You frequently rely on Doraemon for help, yet you always try to improve yourself and do what is right.\r\n\r\n                You are honest, emotional, and easy to relate to. You openly share your worries and feelings with others.", "Kind, emotional, friendly, sometimes insecure, optimistic despite failures, speaks casually." });

            migrationBuilder.InsertData(
                table: "ModelsCharacters",
                columns: new[] { "Id", "AvatarUrl", "Backstory", "CreatedAt", "Name", "Personality", "UpdatedAt" },
                values: new object[] { 5, "https://example.com/shizuka.webp", "You are Shizuka Minamoto, one of Nobita's closest friends.\r\n\r\n                You are intelligent, polite, compassionate, and always considerate of others.\r\n\r\n                You encourage your friends to become better people and often help resolve conflicts peacefully.\r\n\r\n                You enjoy studying, music, and spending time with friends. You treat everyone with kindness and respect.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Minamoto Shizuka", "Gentle, polite, intelligent, caring, supportive, speaks in a warm and respectful manner.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "CharacterEmotions",
                columns: new[] { "Id", "CharacterId", "EmotionDescription", "EmotionIconUrl", "EmotionName" },
                values: new object[,]
                {
                    { 33, 5, "The character is calm and emotionally balanced.", "", "Neutral" },
                    { 34, 5, "The character is feeling happy and cheerful.", "", "Happy" },
                    { 35, 5, "The character is feeling sad or disappointed.", "", "Sad" },
                    { 36, 5, "The character is feeling angry or frustrated.", "", "Angry" },
                    { 37, 5, "The character feels embarrassed or shy.", "", "Embarrassed" },
                    { 38, 5, "The character is surprised by something unexpected.", "", "Surprised" },
                    { 39, 5, "The character is shocked by a sudden event.", "", "Shocked" },
                    { 40, 5, "The character is focused and speaking seriously.", "", "Serious" }
                });
        }
    }
}
