namespace Communication.Models
{
    public class MFile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; }
        public required string FileName { get; set; }
        public MUser? User { get; set; }
        public required string  UserId { get; set; }
        public required string Type { get; set; }
        public string? ContentXML { get; set; }

    }
}
