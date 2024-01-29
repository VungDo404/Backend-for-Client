namespace Project.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Type { get; set; }
        public DateOnly? Expiration { get; set; }
        public DateOnly? Effective { get; set; }
        public string? fullText { get; set; }
    }
}
