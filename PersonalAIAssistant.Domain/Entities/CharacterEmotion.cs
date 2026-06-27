using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalAIAssistant.Domain.Entities
{
    public class CharacterEmotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string EmotionName { get; set; } = string.Empty;
        public string EmotionDescription { get; set; } = string.Empty;
        public string EmotionIconUrl { get; set; } = string.Empty;
        public int CharacterId { get; set; }

        public virtual ModelsCharacter? Character { get; set; }
    }
}
