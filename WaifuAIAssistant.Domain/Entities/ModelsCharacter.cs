namespace WaifuAIAssistant.Domain.Entities
{
    public class ModelsCharacter
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Backstory { get; set; }
        public required string Personality { get; set; } 
        public required string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<CharacterEmotions> CharacterEmotions { get; set; } = new List<CharacterEmotions>();
    }
}
