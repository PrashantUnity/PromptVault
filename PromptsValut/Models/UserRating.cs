namespace PromptsValut.Models;

public class UserRating
{
    public string PromptId { get; set; } =  Guid.NewGuid().ToString();
    public bool Liked { get; set; } = false;
    public int Rating { get; set; } = 0; // 1-5 scale
    public DateTime RatedAt { get; set; } = DateTime.UtcNow;
}
