using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Models;
using PromptsValut.Services;
using PromptsValut.Components.UI;
using PromptsValut.Components.Modals;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PromptsValut.Pages;

public partial class PromptGenerator : ComponentBase, IDisposable
{
    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IPlaceholderParserService PlaceholderParserService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private Prompt promptTemplate = new();
    private string generatedPrompt = string.Empty;
    private string tagsInput = string.Empty;
    private bool isInitialized = false;

    // Modal references
    private PromptDetailModal? promptDetailModal;
    private bool showPromptDetail = false;
    private bool showMultiOptionDialog = false;
    private string multiOptionName = string.Empty;
    private string multiOptionValues = string.Empty;

    // Snackbar properties
    private Snackbar? snackbar;
    private bool showSnackbar = false;
    private string snackbarTitle = string.Empty;
    private string snackbarMessage = string.Empty;
    private SnackbarType snackbarType = SnackbarType.Info;

    protected override async Task OnInitializedAsync()
    {
        // Ensure PromptService is initialized and categories are loaded
        await PromptService.InitializeAsync();
        InitializeTemplate();
        isInitialized = true;
        StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            PromptService.StateChanged += OnStateChanged;
        }
    }

    private void OnStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }


    private void InitializeTemplate()
    {
        promptTemplate = new Prompt
        {
            Title = "Custom Prompt",
            Category = "general",
            Description = "A custom generated prompt",
            Content = "You are a helpful assistant. Please help me with the following task: [task description]",
            Difficulty = "beginner",
            EstimatedTime = "5-10 minutes",
            Author = "Prompt Generator",
            UsageNotes = "Replace [task description] with your specific task description.",
            Placeholders = new[] { "task description" }
        };
        
    }

    private void GeneratePrompt()
    {
        if (string.IsNullOrEmpty(promptTemplate.Content))
        {
            ShowSnackbar("Error", "Please enter prompt content before generating", SnackbarType.Error);
            return;
        }

        // Extract placeholders from content automatically
        var placeholderRegex = new Regex(@"\[([^\]]+)\]");
        var matches = placeholderRegex.Matches(promptTemplate.Content);
        var extractedPlaceholders = matches.Cast<Match>()
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToArray();
        
        promptTemplate.Placeholders = extractedPlaceholders;
        
        // Update tags
        if (!string.IsNullOrEmpty(tagsInput))
        {
            promptTemplate.Tags = tagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim()).ToArray();
        }

        // Serialize the complete prompt template object to JSON (with original content and placeholders)
        generatedPrompt = JsonSerializer.Serialize(promptTemplate, new JsonSerializerOptions { WriteIndented = true });

        ShowSnackbar("Generated!", "Prompt JSON generated successfully", SnackbarType.Success);
    }

    private async Task PreviewPrompt()
    {
        if (string.IsNullOrEmpty(promptTemplate.Content))
        {
            ShowSnackbar("Error", "Please enter prompt content before previewing", SnackbarType.Error);
            return;
        }

        // Create a temporary prompt for preview
        var previewPrompt = new Prompt
        {
            Id = "preview-" + Guid.NewGuid().ToString("N")[..8],
            Title = promptTemplate.Title + " (Preview)",
            Content = promptTemplate.Content,
            Category = promptTemplate.Category,
            Description = promptTemplate.Description,
            Tags = promptTemplate.Tags,
            Author = promptTemplate.Author,
            Difficulty = promptTemplate.Difficulty,
            UsageNotes = promptTemplate.UsageNotes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPublic = false
        };

        // Show the prompt in the existing modal
        if (promptDetailModal != null)
        {
            await promptDetailModal.ShowPrompt(previewPrompt);
            showPromptDetail = true;
            StateHasChanged();
        }
    }

    private async Task ClosePromptDetail()
    {
        showPromptDetail = false;
        await Task.CompletedTask;
    }

    private string GetSampleValue(string placeholder, string fieldType = "text")
    {
        var lowerPlaceholder = placeholder.ToLower();
        
        // Type-specific sample values
        switch (fieldType.ToLower())
        {
            case "email":
                return "user@example.com";
            case "url":
                return "https://example.com";
            case "number":
                return "42";
            case "textarea":
                return "Detailed description here...";
            case "select":
                return GetSelectSampleValue(lowerPlaceholder);
            default:
                return GetTextSampleValue(lowerPlaceholder);
        }
    }

    private string GetTextSampleValue(string placeholder)
    {
        return placeholder switch
        {
            "task" or "task description" => "write a professional email",
            "topic" => "artificial intelligence",
            "tone" => "professional",
            "length" => "200 words",
            "audience" => "business professionals",
            "style" => "formal",
            "format" => "bullet points",
            "language" => "English",
            "purpose" => "informational",
            "context" => "business meeting",
            "company" or "business" => "TechCorp Inc.",
            "product" => "mobile application",
            "service" => "consulting services",
            "industry" => "technology",
            "location" => "San Francisco",
            "date" => "2024-01-15",
            "time" => "2:00 PM",
            "budget" => "$10,000",
            "timeline" => "3 months",
            "goal" or "objective" => "increase user engagement",
            _ => $"sample_{placeholder.Replace(" ", "_")}"
        };
    }

    private string GetSelectSampleValue(string placeholder)
    {
        return placeholder switch
        {
            var p when p.Contains("genre") => "Fiction",
            var p when p.Contains("platform") => "Facebook",
            var p when p.Contains("style") || p.Contains("tone") => "Professional",
            var p when p.Contains("level") || p.Contains("difficulty") => "Beginner",
            var p when p.Contains("frequency") => "Weekly",
            var p when p.Contains("size") => "Medium",
            var p when p.Contains("priority") => "High",
            var p when p.Contains("business type") || p.Contains("company type") => "SaaS",
            var p when p.Contains("industry") => "Technology",
            var p when p.Contains("budget") => "$15K - $50K",
            var p when p.Contains("timeline") || p.Contains("duration") => "1 month",
            var p when p.Contains("goals") || p.Contains("purpose") => "Lead Generation",
            _ => "Option 1"
        };
    }



    private async Task CopyToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", generatedPrompt);
        ShowSnackbar("Copied!", "Prompt JSON copied to clipboard", SnackbarType.Success);
    }

    private void ClearPreview()
    {
        generatedPrompt = string.Empty;
    }


    // Snackbar methods
    private void ShowSnackbar(string title, string message, SnackbarType type)
    {
        snackbarTitle = title;
        snackbarMessage = message;
        snackbarType = type;
        showSnackbar = true;
        StateHasChanged();
    }

    private void OnSnackbarHide()
    {
        showSnackbar = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        PromptService.StateChanged -= OnStateChanged;
    }

    // Interactive Content Methods
    private void InsertText(string before, string after)
    {
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? $"{before}{after}" 
            : $"{promptTemplate.Content}\n{before}{after}";
        promptTemplate.Content = newContent;
        StateHasChanged();
    }

    private void InsertPlaceholder(string placeholderName)
    {
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? $"[{placeholderName}]" 
            : $"{promptTemplate.Content}\n[{placeholderName}]";
        promptTemplate.Content = newContent;
        StateHasChanged();
    }

    private void InsertBulletList()
    {
        var listContent = "• [item 1]\n• [item 2]\n• [item 3]";
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? listContent 
            : $"{promptTemplate.Content}\n{listContent}";
        promptTemplate.Content = newContent;
        StateHasChanged();
    }

    private void InsertNumberedList()
    {
        var listContent = "1. [item 1]\n2. [item 2]\n3. [item 3]";
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? listContent 
            : $"{promptTemplate.Content}\n{listContent}";
        promptTemplate.Content = newContent;
        StateHasChanged();
    }

    private void InsertHeading()
    {
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? "# [heading text]" 
            : $"{promptTemplate.Content}\n# [heading text]";
        promptTemplate.Content = newContent;
        StateHasChanged();
    }

    private void InsertQuote()
    {
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? "> [quote text]" 
            : $"{promptTemplate.Content}\n> [quote text]";
        promptTemplate.Content = newContent;
        StateHasChanged();
    }

    private void OnPlaceholderSelected(ChangeEventArgs e)
    {
        var selectedValue = e.Value?.ToString();
        if (!string.IsNullOrEmpty(selectedValue))
        {
            InsertPlaceholder(selectedValue);
        }
    }

    private void ShowMultiOptionDialog()
    {
        multiOptionName = string.Empty;
        multiOptionValues = string.Empty;
        showMultiOptionDialog = true;
        StateHasChanged();
    }

    private void CloseMultiOptionDialog()
    {
        showMultiOptionDialog = false;
        multiOptionName = string.Empty;
        multiOptionValues = string.Empty;
        StateHasChanged();
    }

    private void InsertMultiOption()
    {
        if (string.IsNullOrEmpty(multiOptionName) || string.IsNullOrEmpty(multiOptionValues))
        {
            ShowSnackbar("Error", "Please enter both placeholder name and options", SnackbarType.Error);
            return;
        }

        var options = multiOptionValues.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(opt => opt.Trim())
            .ToArray();
        
        var placeholder = $"[{multiOptionName}/{string.Join("/", options)}]";
        
        var newContent = string.IsNullOrEmpty(promptTemplate.Content) 
            ? placeholder 
            : $"{promptTemplate.Content}\n{placeholder}";
        promptTemplate.Content = newContent;
        
        CloseMultiOptionDialog();
        ShowSnackbar("Inserted!", "Multi-option placeholder added successfully", SnackbarType.Success);
        StateHasChanged();
    }
}
