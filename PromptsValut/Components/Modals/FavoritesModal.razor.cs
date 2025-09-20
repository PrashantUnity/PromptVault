using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Models;
using PromptsValut.Services;

namespace PromptsValut.Components.Modals;

public partial class FavoritesModal : ComponentBase, IDisposable
{
    [Parameter] public bool IsVisible { get; set; } = false;
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback<Prompt> OnViewPrompt { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<Prompt> favoritePrompts = new();

    protected override async Task OnInitializedAsync()
    {
        PromptService.StateChanged += StateHasChanged;
        await LoadFavorites();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            await LoadFavorites();
        }
    }

    private async Task LoadFavorites()
    {
        favoritePrompts = await PromptService.GetFavoritesAsync();
    }

    private async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }

    private async Task RemoveFromFavorites(string promptId)
    {
        await PromptService.ToggleFavoriteAsync(promptId);
        await LoadFavorites();
    }

    private async Task CopyPrompt(string content)
    {
        await JSRuntime.InvokeVoidAsync("copyToClipboard", content);
    }

    private async Task ViewPrompt(Prompt prompt)
    {
        await OnViewPrompt.InvokeAsync(prompt);
    }

    private string GetCategoryClasses(string category)
    {
        return category switch
        {
            "marketing" => "bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300",
            "development" => "bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300",
            "creative-writing" => "bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-300",
            "business" => "bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300",
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

    public void Dispose()
    {
        PromptService.StateChanged -= StateHasChanged;
    }
}
