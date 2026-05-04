namespace WaifuAIAssistant.Domain.Entities
{
    public class PromptTemplate
    {
        public int Id { get; set; }

        // The unique key for the prompt template, used to identify it in the system.
        public string PromptKey { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}