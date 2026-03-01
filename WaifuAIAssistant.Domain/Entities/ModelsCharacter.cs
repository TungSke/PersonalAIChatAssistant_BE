using Microsoft.EntityFrameworkCore;

namespace WaifuAIAssistant.Domain.Entities
{

    [Index(nameof(Id))]
    public class ModelsCharacter
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Backstory { get; set; }
        public required string Personality { get; set; } 
        public required string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<CharacterEmotion> CharacterEmotions { get; set; } = new List<CharacterEmotion>();
    }
}
