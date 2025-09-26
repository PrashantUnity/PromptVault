using Microsoft.AspNetCore.Components;
using PromptsValut.Constants;
using PromptsValut.Services;

namespace PromptsValut.Components.UI;

public partial class CategoryFilter : ComponentBase, IDisposable
{
    [Parameter] public EventCallback<string> OnCategoryChanged { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        PromptService.StateChanged += StateHasChanged;
        await Task.CompletedTask;
    }

    private async Task SelectCategory(string categoryId)
    {
        await OnCategoryChanged.InvokeAsync(categoryId);
    }

    private string GetCategoryClasses(string categoryId)
    {
        var isActive = PromptService.State.SelectedCategory == categoryId;
        var baseClasses = "w-full flex items-center gap-3 px-3 py-2 rounded-lg text-left transition-all duration-200";
        
        if (isActive)
        {
            return $"{baseClasses} bg-blue-600 text-white shadow-md";
        }
        
        return $"{baseClasses} hover:bg-gray-100 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-gray-100";
    }

    private int GetCategoryCount(string categoryId)
    {
        if (categoryId == "all")
        {
            return PromptService.State.Prompts.Count;
        }

        // Gracefully handle missing categories on prompts or null ids
        return PromptService.State.Prompts.Count(p => !string.IsNullOrEmpty(p.Category) && p.Category == categoryId);
    }

    public void Dispose()
    {
        PromptService.StateChanged -= StateHasChanged;
    }
}
