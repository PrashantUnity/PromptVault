using System.Text.RegularExpressions;

namespace PromptsValut.Services;

public class PlaceholderParserService : IPlaceholderParserService
{
    public ParsedPlaceholders ParsePlaceholders(string content)
    {
        var placeholderRegex = new Regex(@"\[([^\]]+)\]");
        var fields = new List<PlaceholderField>();
        var fieldMap = new Dictionary<string, PlaceholderField>();
        
        var processedContent = content;
        var matches = placeholderRegex.Matches(content);
        
        foreach (Match match in matches)
        {
            var placeholder = match.Groups[1].Value;
            var fieldId = GenerateFieldId(placeholder);
            
            // Skip if we've already processed this placeholder
            if (fieldMap.ContainsKey(fieldId)) continue;
            
            var field = CreateFieldFromPlaceholder(placeholder, fieldId);
            fieldMap[fieldId] = field;
            fields.Add(field);
            
            // Replace placeholder with field reference
            processedContent = processedContent.Replace(match.Value, $"{{{{{fieldId}}}}}");
        }
        
        return new ParsedPlaceholders
        {
            Fields = fields,
            ProcessedContent = processedContent
        };
    }

    public string ReplacePlaceholders(string content, Dictionary<string, string> values)
    {
        var result = content;
        
        foreach (var kvp in values)
        {
            var placeholder = $"{{{{{kvp.Key}}}}}";
            result = result.Replace(placeholder, kvp.Value);
        }
        
        return result;
    }

    private static string GenerateFieldId(string placeholder)
    {
        return placeholder
            .ToLowerInvariant()
            .Replace(" ", "_")
            .Replace("-", "_")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("/", "_")
            .Replace("\\", "_")
            .Replace("|", "_")
            .Replace("+", "_")
            .Replace("=", "_")
            .Replace("@", "_")
            .Replace("#", "_")
            .Replace("$", "_")
            .Replace("%", "_")
            .Replace("^", "_")
            .Replace("&", "_")
            .Replace("*", "_")
            .Replace("(", "_")
            .Replace(")", "_")
            .Trim();
    }

    private static PlaceholderField CreateFieldFromPlaceholder(string placeholder, string fieldId)
    {
        var lowerPlaceholder = placeholder.ToLowerInvariant();
        
        // Determine field type based on placeholder content
        var type = "text";
        var required = true;
        var description = $"Enter {placeholder.ToLowerInvariant()}";
        
        // Check for specific patterns
        if (lowerPlaceholder.Contains("email") || lowerPlaceholder.Contains("e-mail"))
        {
            type = "email";
        }
        else if (lowerPlaceholder.Contains("url") || lowerPlaceholder.Contains("website") || lowerPlaceholder.Contains("link"))
        {
            type = "url";
        }
        else if (lowerPlaceholder.Contains("number") || lowerPlaceholder.Contains("count") || lowerPlaceholder.Contains("age") || lowerPlaceholder.Contains("year"))
        {
            type = "number";
        }
        else if (lowerPlaceholder.Contains("description") || lowerPlaceholder.Contains("details") || lowerPlaceholder.Contains("explain") || lowerPlaceholder.Contains("content"))
        {
            type = "textarea";
        }
        else if (lowerPlaceholder.Contains("choose") || lowerPlaceholder.Contains("select") || lowerPlaceholder.Contains("option"))
        {
            type = "select";
        }
        
        // Check for optional indicators
        if (lowerPlaceholder.Contains("optional") || lowerPlaceholder.Contains("(optional)"))
        {
            required = false;
        }
        
        // Generate options for select fields
        List<string>? options = null;
        if (type == "select")
        {
            options = GenerateSelectOptions(placeholder);
        }
        
        // Check if placeholder contains specific options (e.g., "Professional/Casual/Creative/Technical")
        if (placeholder.Contains('/') && !placeholder.Contains("http"))
        {
            var optionList = placeholder.Split('/').Select(opt => opt.Trim()).ToList();
            if (optionList.Count > 1 && optionList.Count <= 10)
            {
                type = "select";
                options = optionList;
            }
        }
        
        return new PlaceholderField
        {
            Id = fieldId,
            Name = placeholder,
            Type = type,
            Required = required,
            Placeholder = $"Enter {placeholder}",
            Options = options,
            Description = description
        };
    }

    private static List<string> GenerateSelectOptions(string placeholder)
    {
        var lowerPlaceholder = placeholder.ToLowerInvariant();
        
        return lowerPlaceholder switch
        {
            var p when p.Contains("genre") => new List<string> { "Fiction", "Non-fiction", "Mystery", "Romance", "Sci-Fi", "Fantasy", "Thriller", "Horror", "Comedy", "Drama" },
            var p when p.Contains("platform") => new List<string> { "Facebook", "Instagram", "Twitter", "LinkedIn", "TikTok", "YouTube", "Pinterest", "Snapchat" },
            var p when p.Contains("style") || p.Contains("tone") => new List<string> { "Professional", "Casual", "Friendly", "Formal", "Humorous", "Serious", "Creative", "Technical" },
            var p when p.Contains("level") || p.Contains("difficulty") => new List<string> { "Beginner", "Intermediate", "Advanced", "Expert" },
            var p when p.Contains("frequency") => new List<string> { "Daily", "Weekly", "Monthly", "Quarterly", "Annually" },
            var p when p.Contains("size") => new List<string> { "Small", "Medium", "Large", "Extra Large" },
            var p when p.Contains("priority") => new List<string> { "Low", "Medium", "High", "Critical" },
            var p when p.Contains("business type") || p.Contains("company type") => new List<string> { "E-commerce", "SaaS", "Consulting", "Agency", "Non-profit", "Education", "Healthcare", "Finance", "Retail", "Manufacturing" },
            var p when p.Contains("industry") => new List<string> { "Technology", "Healthcare", "Finance", "Education", "Retail", "Manufacturing", "Real Estate", "Food & Beverage", "Travel", "Entertainment" },
            var p when p.Contains("budget") => new List<string> { "Under $5K", "$5K - $15K", "$15K - $50K", "$50K - $100K", "Over $100K" },
            var p when p.Contains("timeline") || p.Contains("duration") => new List<string> { "1-2 weeks", "1 month", "2-3 months", "3-6 months", "6+ months" },
            var p when p.Contains("goals") || p.Contains("purpose") => new List<string> { "Brand Awareness", "Lead Generation", "Sales", "Engagement", "Education", "Support", "Community Building" },
            _ => new List<string> { "Option 1", "Option 2", "Option 3" }
        };
    }
}
