namespace PromptsValut.Models;

public class AppState
{
    public List<Prompt> Prompts { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<string> Favorites { get; set; } = new();
    public Dictionary<string, UserRating> UserRatings { get; set; } = new();
    public List<string> History { get; set; } = new();
    public string SelectedCategory { get; set; } = "all";
    public string SearchQuery { get; set; } = string.Empty;
    public string Theme { get; set; } = "light";
    public bool IsLoading { get; set; } = false;
    public string SortBy { get; set; } = "newest";
    public bool ShowFavoritesOnly { get; set; } = false;
}
