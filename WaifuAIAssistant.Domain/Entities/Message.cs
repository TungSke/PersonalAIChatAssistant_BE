using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int? UserId { get; set; }
        public int? ModelCharacterId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Conversation Conversation { get; set; } = null!;
        public virtual Users Users { get; set; } = null!;
        public virtual ModelsCharacter ModelsCharacter { get; set; } = null!;
    }
}
