using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Models;
using PromptsValut.Services;

namespace PromptsValut.Components.Modals;

public partial class PromptDetailModal : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; } = false;
    [Parameter] public EventCallback OnClose { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IPlaceholderParserService PlaceholderParserService { get; set; } = default!;

    private Prompt? SelectedPrompt { get; set; }
    private bool IsFavorite => SelectedPrompt != null && PromptService.State.Favorites.Contains(SelectedPrompt.Id);
    private int activeTab = 0;
    private Dictionary<string, string> customizeValues = new();
    private ParsedPlaceholders? parsedFields;
    private string? generatedPrompt;
    private bool showGeneratedPrompt = false;
    private Dictionary<string, string> validationErrors = new();
    private bool isEditingPrompt = false;
    private string editedPromptContent = string.Empty;

    public async Task ShowPrompt(Prompt prompt)
    {
        SelectedPrompt = prompt;
        IsVisible = true;
        activeTab = 0; // Reset to Original Prompt tab
        ParsePromptPlaceholders();
        InitializeCustomizeValues();
        showGeneratedPrompt = false; // Reset generated prompt state
        generatedPrompt = null;
        validationErrors.Clear();
        isEditingPrompt = false; // Reset editing state
        editedPromptContent = string.Empty;
        StateHasChanged();
        
        // Small delay to ensure DOM is updated
        await Task.Delay(1);
    }

    private void ParsePromptPlaceholders()
    {
        if (SelectedPrompt != null)
        {
            parsedFields = PlaceholderParserService.ParsePlaceholders(SelectedPrompt.Content);
        }
    }

    private void InitializeCustomizeValues()
    {
        customizeValues.Clear();
        if (parsedFields?.Fields != null)
        {
            foreach (var field in parsedFields.Fields)
            {
                customizeValues[field.Id] = string.Empty;
            }
        }
    }

    private async Task SetActiveTab(int tabIndex)
    {
        activeTab = tabIndex;
        
        // If switching to Customize tab, refresh the parsing to ensure we have the latest content
        if (tabIndex == 1)
        {
            ParsePromptPlaceholders();
            InitializeCustomizeValues();
        }
        
        StateHasChanged();
        
        await Task.Delay(1);
    }

    private async Task CloseModal()
    {
        IsVisible = false;
        SelectedPrompt = null;
        await OnClose.InvokeAsync();
    }

    private async Task CopyContent()
    {
        if (SelectedPrompt != null)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", SelectedPrompt.Content);
        }
    }

    private async Task ToggleFavorite()
    {
        if (SelectedPrompt != null)
        {
            await PromptService.ToggleFavoriteAsync(SelectedPrompt.Id);
        }
    }

    private async Task RatePrompt()
    {
        // TODO: Implement rating modal
        await Task.CompletedTask;
    }

    private void ResetCustomizeForm()
    {
        InitializeCustomizeValues();
        showGeneratedPrompt = false;
        generatedPrompt = null;
        validationErrors.Clear();
        StateHasChanged();
    }

    private async Task GenerateCustomPrompt()
    {
        if (SelectedPrompt == null || parsedFields == null) return;

        // Validate form before generating
        if (!ValidateForm())
        {
            await Task.Delay(1);
            StateHasChanged();
            return;
        }

        // Generate customized prompt by replacing placeholders
        generatedPrompt = PlaceholderParserService.ReplacePlaceholders(parsedFields.ProcessedContent, customizeValues);
        showGeneratedPrompt = true;
        
        await Task.Delay(1); // Small delay to make it truly async
        StateHasChanged();
    }

    private bool ValidateForm()
    {
        validationErrors.Clear();
        bool isValid = true;

        if (parsedFields?.Fields != null)
        {
            foreach (var field in parsedFields.Fields)
            {
                if (field.Required)
                {
                    var value = customizeValues.GetValueOrDefault(field.Id, string.Empty);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        validationErrors[field.Id] = $"{field.Name} is required";
                        isValid = false;
                    }
                }
            }
        }

        return isValid;
    }

    private bool IsFormValid()
    {
        if (parsedFields?.Fields == null) return false;

        foreach (var field in parsedFields.Fields)
        {
            if (field.Required)
            {
                var value = customizeValues.GetValueOrDefault(field.Id, string.Empty);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private async Task CopyOriginalPrompt()
    {
        if (SelectedPrompt != null)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", SelectedPrompt.Content);
        }
    }

    private async Task CopyGeneratedPrompt()
    {
        if (generatedPrompt != null)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", generatedPrompt);
        }
    }

    private string GetInputType(string fieldType)
    {
        return fieldType switch
        {
            "email" => "email",
            "url" => "url",
            "number" => "number",
            _ => "text"
        };
    }

    private string GetCategoryClasses(string category)
    {
        return category switch
        {
            "marketing" => "bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300",
            "development" => "bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300",
            "creative-writing" => "bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-300",
            "business" => "bg-indigo-100 text-indigo-800 dark:bg-indigo-900/20 dark:text-indigo-300",
            "education" => "bg-emerald-100 text-emerald-800 dark:bg-emerald-900/20 dark:text-emerald-300",
            "technology" => "bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300",
            "fun" => "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300",
            "productivity" => "bg-violet-100 text-violet-800 dark:bg-violet-900/20 dark:text-violet-300",
            "data-analysis" => "bg-cyan-100 text-cyan-800 dark:bg-cyan-900/20 dark:text-cyan-300",
            _ => "bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300"
        };
    }

    private string GetDifficultyClasses(string difficulty)
    {
        return difficulty switch
        {
            "beginner" => "bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300",
            "intermediate" => "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300",
            "advanced" => "bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300",
            _ => "bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300"
        };
    }

    private string GetCategoryName(string category)
    {
        return PromptService.State.Categories.FirstOrDefault(c => c.Id == category)?.Name ?? category;
    }

    private async Task ToggleEditMode()
    {
        if (isEditingPrompt)
        {
            // Cancel editing
            CancelEdit();
        }
        else
        {
            // Start editing
            await StartEdit();
        }
    }

    private async Task StartEdit()
    {
        if (SelectedPrompt != null)
        {
            isEditingPrompt = true;
            editedPromptContent = SelectedPrompt.Content;
            StateHasChanged();
            
            await Task.Delay(1);
        }
    }

    private void CancelEdit()
    {
        isEditingPrompt = false;
        editedPromptContent = string.Empty;
        StateHasChanged();
    }

    private async Task SavePrompt()
    {
        if (SelectedPrompt != null && !string.IsNullOrWhiteSpace(editedPromptContent))
        {
            // Update the prompt content
            SelectedPrompt.Content = editedPromptContent.Trim();
            
            // Save to service (this will update in memory, storage, and notify state change)
            await PromptService.UpdatePromptAsync(SelectedPrompt);
            
            // Re-parse placeholders with updated content
            ParsePromptPlaceholders();
            InitializeCustomizeValues();
            
            // Exit edit mode
            isEditingPrompt = false;
            editedPromptContent = string.Empty;
            StateHasChanged();
        }
    }

    private async Task CopyEditedPrompt()
    {
        if (!string.IsNullOrEmpty(editedPromptContent))
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", editedPromptContent);
        }
    }
}
