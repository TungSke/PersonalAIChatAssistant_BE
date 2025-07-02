using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? WaifuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Users User { get; set; } = null!;
        public virtual ModelsCharacter? Waifu { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();    
    }
}
