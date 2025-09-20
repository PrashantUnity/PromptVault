using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Models;
using PromptsValut.Services;

namespace PromptsValut.Components.UI;

public partial class PromptCard : ComponentBase, IDisposable
{
    [Parameter] public Prompt Prompt { get; set; } = new();
    [Parameter] public EventCallback<string> OnPromptClick { get; set; }
    [Parameter] public EventCallback<string> OnToggleFavorite { get; set; }
    [Parameter] public EventCallback<(string promptId, UserRating rating)> OnSetRating { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private bool IsFavorite => PromptService.State.Favorites.Contains(Prompt.Id);
    private int UserRating => PromptService.State.UserRatings.ContainsKey(Prompt.Id) ? PromptService.State.UserRatings[Prompt.Id].Rating : 0;

    protected override async Task OnInitializedAsync()
    {
        PromptService.StateChanged += StateHasChanged;
        await Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Icons are now SVG-based, no initialization needed
            await Task.CompletedTask;
        }
    }

    private async Task ToggleFavorite()
    {
        await OnToggleFavorite.InvokeAsync(Prompt.Id);
    }

    private async Task CopyPrompt()
    {
        await JSRuntime.InvokeVoidAsync("copyToClipboard", Prompt.Content);
    }

    private async Task ViewPrompt()
    {
        await OnPromptClick.InvokeAsync(Prompt.Id);
    }

    private async Task SetRating(int rating)
    {
        var userRating = new UserRating
        {
            PromptId = Prompt.Id,
            Rating = rating,
            Liked = rating >= 4
        };
        
        await OnSetRating.InvokeAsync((Prompt.Id, userRating));
    }

    private string GetCategoryGradientClasses(string category)
    {
        return category switch
        {
            "marketing" => "bg-gradient-to-br from-pink-100/80 via-pink-200/80 to-purple-200/80 dark:from-pink-900/30 dark:via-pink-800/30 dark:to-purple-800/30",
            "development" => "bg-gradient-to-br from-green-100/80 via-emerald-200/80 to-blue-200/80 dark:from-green-900/30 dark:via-emerald-800/30 dark:to-blue-800/30",
            "creative-writing" => "bg-gradient-to-br from-purple-100/80 via-purple-200/80 to-pink-200/80 dark:from-purple-900/30 dark:via-purple-800/30 dark:to-pink-800/30",
            "business" => "bg-gradient-to-br from-blue-100/80 via-blue-200/80 to-indigo-200/80 dark:from-blue-900/30 dark:via-blue-800/30 dark:to-indigo-800/30",
            "education" => "bg-gradient-to-br from-emerald-100/80 via-green-200/80 to-teal-200/80 dark:from-emerald-900/30 dark:via-green-800/30 dark:to-teal-800/30",
            "technology" => "bg-gradient-to-br from-orange-100/80 via-orange-200/80 to-red-200/80 dark:from-orange-900/30 dark:via-orange-800/30 dark:to-red-800/30",
            "fun" => "bg-gradient-to-br from-yellow-100/80 via-yellow-200/80 to-orange-200/80 dark:from-yellow-900/30 dark:via-yellow-800/30 dark:to-orange-800/30",
            "productivity" => "bg-gradient-to-br from-violet-100/80 via-purple-200/80 to-indigo-200/80 dark:from-violet-900/30 dark:via-purple-800/30 dark:to-indigo-800/30",
            "data-analysis" => "bg-gradient-to-br from-cyan-100/80 via-blue-200/80 to-indigo-200/80 dark:from-cyan-900/30 dark:via-blue-800/30 dark:to-indigo-800/30",
            _ => "bg-gradient-to-br from-gray-100/80 via-gray-200/80 to-gray-300/80 dark:from-gray-800/30 dark:via-gray-700/30 dark:to-gray-600/30"
        };
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

    private string GetCategoryTagClasses(string category)
    {
        return category switch
        {
            "marketing" => "bg-pink-200 text-pink-800 dark:bg-pink-900/30 dark:text-pink-300",
            "development" => "bg-green-200 text-green-800 dark:bg-green-900/30 dark:text-green-300",
            "creative-writing" => "bg-purple-200 text-purple-800 dark:bg-purple-900/30 dark:text-purple-300",
            "business" => "bg-blue-200 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300",
            "education" => "bg-emerald-200 text-emerald-800 dark:bg-emerald-900/30 dark:text-emerald-300",
            "technology" => "bg-orange-200 text-orange-800 dark:bg-orange-900/30 dark:text-orange-300",
            "fun" => "bg-yellow-200 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300",
            "productivity" => "bg-violet-200 text-violet-800 dark:bg-violet-900/30 dark:text-violet-300",
            "data-analysis" => "bg-cyan-200 text-cyan-800 dark:bg-cyan-900/30 dark:text-cyan-300",
            _ => "bg-gray-200 text-gray-800 dark:bg-gray-800 dark:text-gray-300"
        };
    }

    private string GetDifficultyTagClasses(string difficulty)
    {
        return difficulty switch
        {
            "beginner" => "bg-green-200 text-green-800 dark:bg-green-900/30 dark:text-green-300",
            "intermediate" => "bg-yellow-200 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300",
            "advanced" => "bg-red-200 text-red-800 dark:bg-red-900/30 dark:text-red-300",
            _ => "bg-gray-200 text-gray-800 dark:bg-gray-800 dark:text-gray-300"
        };
    }

    public void Dispose()
    {
        PromptService.StateChanged -= StateHasChanged;
    }
}
