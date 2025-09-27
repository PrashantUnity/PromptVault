namespace PromptsValut.Models;

public class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int PromptCount { get; set; } = 0;
    public int SortOrder { get; set; } = 0;
    public string FilePathName { get; set; } = string.Empty;
}
