using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaifuAIAssistant.Domain.Entities
{

    [Index(nameof(Id))]
    public class ModelsCharacter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Backstory { get; set; }
        public required string Personality { get; set; } 
        public required string AvatarUrl { get; set; }
        public required string SpeakingStyle { get; set; } = string.Empty;
        public required string IntelligenceLevel { get; set; } = string.Empty;
        public required string ResponseStyle { get; set; } = string.Empty;
        public required string ExampleDialogue { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<CharacterEmotion> CharacterEmotions { get; set; } = new List<CharacterEmotion>();
    }
}
