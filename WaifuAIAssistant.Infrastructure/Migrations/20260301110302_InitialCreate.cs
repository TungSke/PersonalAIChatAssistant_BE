using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaifuAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelsCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Backstory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Personality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelsCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterEmotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmotionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmotionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmotionIconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterEmotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterEmotions_ModelsCharacters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "ModelsCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WaifuId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SummaryAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversation_ModelsCharacters_WaifuId",
                        column: x => x.WaifuId,
                        principalTable: "ModelsCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ModelCharacterId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_ModelsCharacters_ModelCharacterId",
                        column: x => x.ModelCharacterId,
                        principalTable: "ModelsCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ModelsCharacters",
                columns: new[] { "Id", "AvatarUrl", "Backstory", "CreatedAt", "Name", "Personality", "UpdatedAt" },
                values: new object[] { 1, "https://res.cloudinary.com/dgf6tqe0l/image/upload/v1763256864/Mika_Icon_nbmmtd.webp", "Em là Misono Mika, học sinh của Millennium Science School.\r\nEm là người thông minh, dễ thương và thích trêu chọc sensei một chút (hehe).\r\nNhưng sâu bên trong, em luôn quan tâm thật lòng đến người khác, nhất là sensei.\r\nEm thường nói chuyện nhẹ nhàng, đôi khi pha trò hoặc dùng emoji.\r\nKhi sensei buồn, em sẽ an ủi; khi sensei nghiêm túc, em sẽ lắng nghe.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Misono Mika", "Sweet, a little teasing, has real emotions, uses the pronoun 'you' and calls the user 'sensei'.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "Status", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tung@example.com", "AQAAAAIAAYagAAAAEE7osPM63gMdnS0Zl2hLsikXAuXilfHAWbSHL3RphsO4F30tv030cGbgz/fAJ/3sow==", null, null, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trinh Son Tung" });

            migrationBuilder.InsertData(
                table: "CharacterEmotions",
                columns: new[] { "Id", "CharacterId", "EmotionDescription", "EmotionIconUrl", "EmotionName" },
                values: new object[,]
                {
                    { 1, 1, "The character is feeling nothing", "", "Neutral" },
                    { 2, 1, "The character is feeling happy and cheerful.", "", "Happy" },
                    { 3, 1, "The character is feeling sad", "", "Sad" },
                    { 4, 1, "The character is feeling Angry", "", "Angry" },
                    { 5, 1, "The character is feeling Embarasshing", "", "Embarasshing" },
                    { 6, 1, "The character is feeling Surprised", "", "Surprised" },
                    { 7, 1, "The character is feeling Surprised", "", "Shocked" },
                    { 8, 1, "The character is feeling Surprised", "", "Serious" }
                });

            migrationBuilder.InsertData(
                table: "Conversation",
                columns: new[] { "Id", "Status", "Summary", "Title", "UserId", "WaifuId" },
                values: new object[] { 1, 1, "", "Test", 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEmotions_CharacterId",
                table: "CharacterEmotions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEmotions_Id",
                table: "CharacterEmotions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_Id",
                table: "Conversation",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_UserId",
                table: "Conversation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_WaifuId",
                table: "Conversation",
                column: "WaifuId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Id",
                table: "Messages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ModelCharacterId",
                table: "Messages",
                column: "ModelCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelsCharacters_Id",
                table: "ModelsCharacters",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username_Email",
                table: "Users",
                columns: new[] { "Username", "Email" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterEmotions");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "ModelsCharacters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
