namespace PromptsValut.Models;

public class ExportData
{
    public List<Prompt> Prompts { get; set; } = new();
    public List<string> Favorites { get; set; } = new();
    public Dictionary<string, UserRating> UserRatings { get; set; } = new();
    public List<string> History { get; set; } = new();
    public string ExportDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}
