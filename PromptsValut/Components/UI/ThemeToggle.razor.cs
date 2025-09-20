using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Services;

namespace PromptsValut.Components.UI;

public partial class ThemeToggle : ComponentBase, IDisposable
{
    [Parameter] public bool IsMobile { get; set; } = false;
    [Parameter] public EventCallback OnToggleTheme { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        PromptService.StateChanged += StateHasChanged;
        await Task.CompletedTask;
    }

    private async Task ToggleTheme()
    {
        await OnToggleTheme.InvokeAsync();
    }

    public void Dispose()
    {
        PromptService.StateChanged -= StateHasChanged;
    }
}
