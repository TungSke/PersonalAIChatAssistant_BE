using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Application.DTOs.Response
{
    public class ConversationResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? WaifuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}