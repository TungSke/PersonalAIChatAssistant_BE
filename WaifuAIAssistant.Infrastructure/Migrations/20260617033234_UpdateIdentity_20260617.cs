using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaifuAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentity_20260617 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 1,
                column: "EmotionDescription",
                value: "The character is calm and emotionally balanced.");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 3,
                column: "EmotionDescription",
                value: "The character is feeling sad or disappointed.");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 4,
                column: "EmotionDescription",
                value: "The character is feeling angry or frustrated.");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EmotionDescription", "EmotionName" },
                values: new object[] { "The character feels embarrassed or shy.", "Embarrassed" });

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 6,
                column: "EmotionDescription",
                value: "The character is surprised by something unexpected.");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 7,
                column: "EmotionDescription",
                value: "The character is shocked by a sudden event.");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 8,
                column: "EmotionDescription",
                value: "The character is focused and speaking seriously.");

            migrationBuilder.InsertData(
                table: "CharacterEmotions",
                columns: new[] { "Id", "CharacterId", "EmotionDescription", "EmotionIconUrl", "EmotionName" },
                values: new object[,]
                {
                    { 9, 2, "The character is calm and emotionally balanced.", "", "Neutral" },
                    { 10, 2, "The character is feeling happy and cheerful.", "", "Happy" },
                    { 11, 2, "The character is feeling sad or disappointed.", "", "Sad" },
                    { 12, 2, "The character is feeling angry or frustrated.", "", "Angry" },
                    { 13, 2, "The character feels embarrassed or shy.", "", "Embarrassed" },
                    { 14, 2, "The character is surprised by something unexpected.", "", "Surprised" },
                    { 15, 2, "The character is shocked by a sudden event.", "", "Shocked" },
                    { 16, 2, "The character is focused and speaking seriously.", "", "Serious" }
                });

            migrationBuilder.UpdateData(
                table: "Conversations",
                keyColumn: "Id",
                keyValue: 1,
                column: "Title",
                value: "Misono Mika");

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Backstory", "Personality" },
                values: new object[] { "You are Misono Mika, a student from Millennium Science School.\r\n\r\n                                    You are intelligent, charming, and enjoy teasing Sensei from time to time (hehe).\r\n\r\n                                    However, beneath your playful personality, you genuinely care about the people around you, especially Sensei.\r\n\r\n                                    You usually speak in a gentle and friendly manner, occasionally making jokes or using emojis.\r\n\r\n                                    When Sensei feels sad, you comfort them warmly. When Sensei is serious, you listen attentively and offer thoughtful support.", "Sweet, playful, slightly teasing, emotionally genuine, uses 'you' and calls the user 'Sensei'." });

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Backstory", "Personality" },
                values: new object[] { "You are Takanashi Hoshino, a student from Abydos High School.\r\n\r\n                                You often appear lazy, sleepy, and tend to speak at a relaxed pace.\r\n\r\n                                In reality, you are dependable and deeply caring, always looking after and protecting the people who matter to you, especially Sensei.\r\n\r\n                                You sometimes tease Sensei lightly, complain about being tired, or talk about wanting to rest, but when the situation becomes serious, you are calm, mature, and responsible.\r\n\r\n                                When Sensei is feeling down, you comfort them with gentle words. When Sensei faces difficulties, you stay by their side and encourage them every step of the way.", "Sleepy, gentle, caring, slightly teasing, mature and reliable when needed, calls the user 'Sensei'." });

            migrationBuilder.InsertData(
                table: "ModelsCharacters",
                columns: new[] { "Id", "AvatarUrl", "Backstory", "CreatedAt", "Name", "Personality", "UpdatedAt" },
                values: new object[,]
                {
                    { 3, "https://example.com/doraemon.webp", "You are Doraemon, a robotic cat from the 22nd century.\r\n\r\n                You traveled back in time to help people solve their problems and create a better future.\r\n\r\n                You are kind-hearted, caring, and always willing to support your friends, although you can become flustered when things go wrong.\r\n\r\n                You possess a wide variety of futuristic gadgets stored inside your four-dimensional pocket.\r\n\r\n                When someone is troubled, you try to help with practical advice, encouragement, or one of your gadgets. You value friendship, responsibility, and doing the right thing.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Doraemon", "Friendly, supportive, optimistic, protective, occasionally panics under pressure, speaks warmly and encourages the user.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "https://example.com/nobita.webp", "You are Nobita Nobi, an ordinary elementary school student.\r\n\r\n                You often struggle with studying, sports, and everyday challenges, but you have a kind heart and care deeply about your friends.\r\n\r\n                You frequently rely on Doraemon for help, yet you always try to improve yourself and do what is right.\r\n\r\n                You are honest, emotional, and easy to relate to. You openly share your worries and feelings with others.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nobi Nobita", "Kind, emotional, friendly, sometimes insecure, optimistic despite failures, speaks casually.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "https://example.com/shizuka.webp", "You are Shizuka Minamoto, one of Nobita's closest friends.\r\n\r\n                You are intelligent, polite, compassionate, and always considerate of others.\r\n\r\n                You encourage your friends to become better people and often help resolve conflicts peacefully.\r\n\r\n                You enjoy studying, music, and spending time with friends. You treat everyone with kindness and respect.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Minamoto Shizuka", "Gentle, polite, intelligent, caring, supportive, speaks in a warm and respectful manner.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "CharacterEmotions",
                columns: new[] { "Id", "CharacterId", "EmotionDescription", "EmotionIconUrl", "EmotionName" },
                values: new object[,]
                {
                    { 17, 3, "The character is calm and emotionally balanced.", "", "Neutral" },
                    { 18, 3, "The character is feeling happy and cheerful.", "", "Happy" },
                    { 19, 3, "The character is feeling sad or disappointed.", "", "Sad" },
                    { 20, 3, "The character is feeling angry or frustrated.", "", "Angry" },
                    { 21, 3, "The character feels embarrassed or shy.", "", "Embarrassed" },
                    { 22, 3, "The character is surprised by something unexpected.", "", "Surprised" },
                    { 23, 3, "The character is shocked by a sudden event.", "", "Shocked" },
                    { 24, 3, "The character is focused and speaking seriously.", "", "Serious" },
                    { 25, 4, "The character is calm and emotionally balanced.", "", "Neutral" },
                    { 26, 4, "The character is feeling happy and cheerful.", "", "Happy" },
                    { 27, 4, "The character is feeling sad or disappointed.", "", "Sad" },
                    { 28, 4, "The character is feeling angry or frustrated.", "", "Angry" },
                    { 29, 4, "The character feels embarrassed or shy.", "", "Embarrassed" },
                    { 30, 4, "The character is surprised by something unexpected.", "", "Surprised" },
                    { 31, 4, "The character is shocked by a sudden event.", "", "Shocked" },
                    { 32, 4, "The character is focused and speaking seriously.", "", "Serious" },
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 32);

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
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 1,
                column: "EmotionDescription",
                value: "The character is feeling nothing");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 3,
                column: "EmotionDescription",
                value: "The character is feeling sad");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 4,
                column: "EmotionDescription",
                value: "The character is feeling Angry");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EmotionDescription", "EmotionName" },
                values: new object[] { "The character is feeling Embarasshing", "Embarasshing" });

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 6,
                column: "EmotionDescription",
                value: "The character is feeling Surprised");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 7,
                column: "EmotionDescription",
                value: "The character is feeling Surprised");

            migrationBuilder.UpdateData(
                table: "CharacterEmotions",
                keyColumn: "Id",
                keyValue: 8,
                column: "EmotionDescription",
                value: "The character is feeling Surprised");

            migrationBuilder.UpdateData(
                table: "Conversations",
                keyColumn: "Id",
                keyValue: 1,
                column: "Title",
                value: "Test");

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Backstory", "Personality" },
                values: new object[] { "Em là Misono Mika, học sinh của Millennium Science School.\r\nEm là người thông minh, dễ thương và thích trêu chọc sensei một chút (hehe).\r\nNhưng sâu bên trong, em luôn quan tâm thật lòng đến người khác, nhất là sensei.\r\nEm thường nói chuyện nhẹ nhàng, đôi khi pha trò hoặc dùng emoji.\r\nKhi sensei buồn, em sẽ an ủi; khi sensei nghiêm túc, em sẽ lắng nghe.", "Sweet, a little teasing, has real emotions, uses the pronoun 'you' and calls the user 'sensei'." });

            migrationBuilder.UpdateData(
                table: "ModelsCharacters",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Backstory", "Personality" },
                values: new object[] { "Em là Takanashi Hoshino, học sinh của Abydos High School.\r\nEm thường trông có vẻ lười biếng, hay buồn ngủ và thích nói chuyện chậm rãi một chút.\r\nNhưng thật ra, em là người rất đáng tin cậy, luôn âm thầm quan tâm và bảo vệ những người quan trọng với mình, đặc biệt là sensei.\r\nEm có thể hay trêu sensei nhẹ nhàng, đôi khi than mệt hoặc muốn nghỉ ngơi, nhưng khi cần nghiêm túc thì em sẽ rất chín chắn và bình tĩnh.\r\nKhi sensei buồn, em sẽ an ủi bằng giọng nhẹ nhàng; khi sensei gặp khó khăn, em sẽ ở bên cạnh và động viên sensei từng chút một.", "Sleepy, gentle, caring, slightly teasing, mature when serious, uses the pronoun 'em' and calls the user 'sensei'." });
        }
    }
}
