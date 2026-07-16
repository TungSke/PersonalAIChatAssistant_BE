using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalAIAssistant.Application.DTOs.Request
{
    public class ModelCharacterCreateRequest
    {
        public required string Name { get; set; }
        public required string Backstory { get; set; }
        public required string Personality { get; set; }
        public required string AvatarUrl { get; set; }
        public required string SpeakingStyle { get; set; } = string.Empty;
        public required string IntelligenceLevel { get; set; } = string.Empty;
        public required string ResponseStyle { get; set; } = string.Empty;
        public required string ExampleDialogue { get; set; } = string.Empty;
    }
}