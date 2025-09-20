using PromptsValut.Models;

namespace PromptsValut.Services;

public interface IPlaceholderParserService
{
    ParsedPlaceholders ParsePlaceholders(string content);
    string ReplacePlaceholders(string content, Dictionary<string, string> values);
}

public class PlaceholderField
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "text"; // text, textarea, select, number, email, url
    public bool Required { get; set; } = true;
    public string? Placeholder { get; set; }
    public List<string>? Options { get; set; }
    public string? Description { get; set; }
}

public class ParsedPlaceholders
{
    public List<PlaceholderField> Fields { get; set; } = new();
    public string ProcessedContent { get; set; } = string.Empty;
}
