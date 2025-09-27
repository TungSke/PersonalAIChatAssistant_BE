using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaifuAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelCharacters",
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
                    table.PrimaryKey("PK_ModelCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_CharacterEmotions_ModelCharacters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "ModelCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WaifuId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_ModelCharacters_WaifuId",
                        column: x => x.WaifuId,
                        principalTable: "ModelCharacters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_Users_UserId",
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
                        name: "FK_Messages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_ModelCharacters_ModelCharacterId",
                        column: x => x.ModelCharacterId,
                        principalTable: "ModelCharacters",
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
                table: "ModelCharacters",
                columns: new[] { "Id", "AvatarUrl", "Backstory", "CreatedAt", "Name", "Personality", "UpdatedAt" },
                values: new object[] { 1, "https://example.com/default-character.png", "This is a default character for the application.", new DateTime(2025, 9, 27, 7, 24, 9, 307, DateTimeKind.Utc).AddTicks(7663), "Misono Mika", "Friendly and helpful", new DateTime(2025, 9, 27, 7, 24, 9, 307, DateTimeKind.Utc).AddTicks(7664) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "Status", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2025, 9, 27, 7, 24, 9, 307, DateTimeKind.Utc).AddTicks(7553), "tung@example.com", "$2a$11$eW5z1Z3b1Q8f5k5j5k5j5uO5z1Z3b1Q8f5k5j5k5j5uO5z1Z3b1Q8", null, null, "Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trinh Son Tung" });

            migrationBuilder.InsertData(
                table: "CharacterEmotions",
                columns: new[] { "Id", "CharacterId", "EmotionDescription", "EmotionIconUrl", "EmotionName" },
                values: new object[,]
                {
                    { 1, 1, "The character is feeling happy and cheerful.", "", "Happy" },
                    { 2, 1, "The character is feeling sad", "", "Sad" },
                    { 3, 1, "The character is feeling Angry", "", "Angry" },
                    { 4, 1, "The character is feeling Embarasshing", "", "Embarasshing" },
                    { 5, 1, "The character is feeling Hatred", "", "Hatred" }
                });

            migrationBuilder.InsertData(
                table: "Conversations",
                columns: new[] { "Id", "CreatedAt", "Title", "UpdatedAt", "UserId", "WaifuId" },
                values: new object[] { 1, new DateTime(2025, 9, 27, 14, 24, 9, 307, DateTimeKind.Local).AddTicks(7715), "Test", new DateTime(2025, 9, 27, 14, 24, 9, 307, DateTimeKind.Local).AddTicks(7725), 1, 1 });

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
                name: "IX_Conversations_Id",
                table: "Conversations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_WaifuId",
                table: "Conversations",
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
                name: "IX_ModelCharacters_Id",
                table: "ModelCharacters",
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
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "ModelCharacters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
