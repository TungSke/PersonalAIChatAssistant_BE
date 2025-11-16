using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Application.DTOs.Response
{
    public class ModelCharacterResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
