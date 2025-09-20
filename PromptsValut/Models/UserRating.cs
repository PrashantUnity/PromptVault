namespace PromptsValut.Models;

public class UserRating
{
    public string PromptId { get; set; } = string.Empty;
    public bool Liked { get; set; } = false;
    public int Rating { get; set; } = 0; // 1-5 scale
    public DateTime RatedAt { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }
}
