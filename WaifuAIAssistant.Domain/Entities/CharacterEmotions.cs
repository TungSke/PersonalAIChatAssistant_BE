namespace WaifuAIAssistant.Domain.Entities
{
    public class CharacterEmotions
    {
        public Guid Id { get; set; }
        public string EmotionName { get; set; } = string.Empty;
        public string EmotionDescription { get; set; } = string.Empty;
        public string EmotionIconUrl { get; set; } = string.Empty;
        public Guid CharacterId { get; set; }

        public virtual ModelsCharacter? Character { get; set; }
    }
}
