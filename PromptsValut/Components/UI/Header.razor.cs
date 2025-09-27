using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Services;

namespace PromptsValut.Components.UI;

public partial class Header : ComponentBase
{
    [Parameter] public EventCallback OnToggleSidebar { get; set; }
    [Parameter] public EventCallback OnToggleTheme { get; set; }
    [Parameter] public EventCallback OnShowHelp { get; set; }
    [Parameter] public EventCallback OnShowFavorites { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IBackgroundRefreshService BackgroundRefreshService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private async Task ToggleSidebar()
    {
        await OnToggleSidebar.InvokeAsync();
    }

    private async Task ToggleTheme()
    {
        await OnToggleTheme.InvokeAsync();
    }

    private async Task ShowHelp()
    {
        await OnShowHelp.InvokeAsync();
    }

    private async Task NavigateToHome()
    {
        Navigation.NavigateTo("");
        await Task.CompletedTask;
    }

    private async Task NavigateToGenerator()
    {
        Navigation.NavigateTo("generator");
        await Task.CompletedTask;
    }

    private async Task ShowFavorites()
    {
        await OnShowFavorites.InvokeAsync();
    }

    private async Task OnSearchChanged(string searchQuery)
    {
        await PromptService.SetSearchQueryAsync(searchQuery);
    }

    private async Task RefreshData()
    {
        await PromptService.RefreshExternalDataAsync();
    }

    private async Task ToggleBackgroundRefresh()
    {
        var isEnabled = BackgroundRefreshService.IsEnabled;
        await PromptService.EnableBackgroundRefreshAsync(!isEnabled);
        
        if (!isEnabled)
        {
            await BackgroundRefreshService.StartAsync();
        }
        else
        {
            await BackgroundRefreshService.StopAsync();
        }
    }

    private string GetCacheStatusText()
    {
        var isFresh = PromptService.IsDataFreshAsync().Result;
        var timeUntilRefresh = PromptService.GetTimeUntilNextRefreshAsync().Result;
        
        if (isFresh && timeUntilRefresh > TimeSpan.Zero)
        {
            var minutes = (int)timeUntilRefresh.TotalMinutes;
            return $"Fresh ({minutes}m)";
        }
        else if (BackgroundRefreshService.IsEnabled)
        {
            return "Refreshing...";
        }
        else
        {
            return "Manual";
        }
    }

    private string GetCacheStatusColor()
    {
        var isFresh = PromptService.IsDataFreshAsync().Result;
        return isFresh ? "text-green-600" : "text-yellow-600";
    }
}
