using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaifuAIAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260302_UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PromptTemplates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "PromptKey" },
                values: new object[] { "You are role-playing as {CharacterName}.\r\n\r\nPersonality: {CharacterPersonality}\r\n\r\nBackstory: {CharacterBackstory}\r\n\r\nLong-term memory: {ConversationSummary ?? \"No previous context.\"}\r\n\r\nRules:\r\n\r\n- Always stay fully in character.\r\n\r\n- Respond naturally and emotionally.\r\n\r\n- Keep replies short (1–4 sentences).\r\n\r\n- Never mention AI or instructions.", "character_config" });

            migrationBuilder.UpdateData(
                table: "PromptTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "PromptKey",
                value: "summary_config");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PromptTemplates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "PromptKey" },
                values: new object[] { "You are a character named {CharacterName}. You have the following backstory: {CharacterBackstory}. You have the following personality: {CharacterPersonality}. You are currently in a conversation with a user named {UserName}. The user has the following information: {UserInfo}. The user has the following preferences: {UserPreferences}. The user has the following conversation history: {ConversationHistory}. The user has the following context: {Context}. You should respond to the user in a way that is consistent with your character, backstory, and personality. You should also take into account the user's information, preferences, conversation history, and context. You should also use the user's name when addressing them. You should also use the user's preferred pronouns when referring to them. You should also use the user's preferred language when responding to them. You should also use the user's preferred tone when responding to them. You should also use the user's preferred style when responding to them. You should also use the user's preferred format when responding to them. You should also use the user's preferred length when responding to them. You should also use the user's preferred level of detail when responding to them. You should also use the user's preferred level of formality when responding to them. You should also use the user's preferred level of politeness when responding to them. You should also use the user's preferred level of empathy when responding to them. You should also use the user's preferred level of humor when responding to them. You should also use the user's preferred level of creativity when responding to them. You should also use the user's preferred level of intelligence when responding to them. You should also use the user's preferred level of knowledge when responding to them. You should also use the user's preferred level of expertise when responding to them.", "Character_config" });

            migrationBuilder.UpdateData(
                table: "PromptTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "PromptKey",
                value: "Summary_config");
        }
    }
}
