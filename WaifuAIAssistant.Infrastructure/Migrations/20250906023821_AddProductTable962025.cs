using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaifuAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable962025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "ModelsCharacterId");

            migrationBuilder.AddColumn<int>(
                name: "ModelCharacterId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ModelCharacters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 6, 2, 38, 21, 175, DateTimeKind.Utc).AddTicks(4892), new DateTime(2025, 9, 6, 2, 38, 21, 175, DateTimeKind.Utc).AddTicks(4893) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 2, 38, 21, 175, DateTimeKind.Utc).AddTicks(4797));

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ModelsCharacterId",
                table: "Messages",
                column: "ModelsCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ModelCharacters_ModelsCharacterId",
                table: "Messages",
                column: "ModelsCharacterId",
                principalTable: "ModelCharacters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ModelCharacters_ModelsCharacterId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ModelsCharacterId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ModelCharacterId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ModelsCharacterId",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.UpdateData(
                table: "ModelCharacters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 9, 56, 4, 707, DateTimeKind.Utc).AddTicks(5541));
        }
    }
}
