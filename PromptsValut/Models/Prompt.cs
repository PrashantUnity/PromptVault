namespace PromptsValut.Models;

public class Prompt
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public int UsageCount { get; set; } = 0;
    public double AverageRating { get; set; } = 0.0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; } = true;
    public string Author { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "beginner"; // beginner, intermediate, advanced
    public string[] Placeholders { get; set; } = Array.Empty<string>();
    public string UsageNotes { get; set; } = string.Empty;
    public string EstimatedTime { get; set; } = string.Empty;
}
