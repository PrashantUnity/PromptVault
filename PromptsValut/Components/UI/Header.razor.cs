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
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

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
}
