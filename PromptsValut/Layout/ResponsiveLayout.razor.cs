using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Services;
using PromptsValut.Components.UI;

namespace PromptsValut.Layout;

public partial class ResponsiveLayout : LayoutComponentBase, IDisposable
{
    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private Header? header;
    private CategoryFilter? categoryFilter;
    private bool isMobile = false;
    private bool isInitialized = false;
    private bool sidebarOpen = false;

    protected override async Task OnInitializedAsync()
    {
        PromptService.StateChanged += StateHasChanged;
        await CheckScreenSize();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("addResizeListener", DotNetObjectReference.Create(this));
            await CheckScreenSize();
        }
    }

    [JSInvokable]
    public async Task OnResize()
    {
        await CheckScreenSize();
    }

    private async Task CheckScreenSize()
    {
        try
        {
            var width = await JSRuntime.InvokeAsync<int>("getWindowWidth");
            var wasMobile = isMobile;
            isMobile = width < 768; // Mobile breakpoint at 768px
            
            if (wasMobile != isMobile || !isInitialized)
            {
                isInitialized = true;
                StateHasChanged();
            }
        }
        catch
        {
            // Fallback to desktop if JS interop fails
            isMobile = false;
            if (!isInitialized)
            {
                isInitialized = true;
                StateHasChanged();
            }
        }
    }

    private void ToggleSidebar()
    {
        sidebarOpen = !sidebarOpen;
        StateHasChanged();
    }

    private async Task ToggleTheme()
    {
        await PromptService.ToggleThemeAsync();
    }

    private async Task ShowHelp()
    {
        await PromptService.ShowHelpModalAsync();
    }

    private async Task OnSearchChanged(string searchQuery)
    {
        await PromptService.SetSearchQueryAsync(searchQuery);
    }

    private async Task OnCategoryChanged(string category)
    {
        await PromptService.SetSelectedCategoryAsync(category);
        // Close sidebar after category selection on mobile
        if (isMobile)
        {
            sidebarOpen = false;
            StateHasChanged();
        }
    }

    private async Task ShowFavorites()
    {
        if (isMobile)
        {
            // Mobile: Filter to show only favorites
            await PromptService.SetShowFavoritesOnlyAsync(true);
            sidebarOpen = false;
            StateHasChanged();
        }
        else
        {
            // Desktop: Show favorites modal
            await PromptService.ShowFavoritesModalAsync();
        }
    }

    private async Task ShowHistory()
    {
        // This would show history - you might need to implement this in PromptService
        if (isMobile)
        {
            sidebarOpen = false;
            StateHasChanged();
        }
        await Task.CompletedTask;
    }

    private async Task ShowAllPrompts()
    {
        await PromptService.SetShowFavoritesOnlyAsync(false);
        await PromptService.SetSelectedCategoryAsync("all");
    }

    private void ShowSearch()
    {
        // Focus on search input - you might need to implement this
        if (isMobile)
        {
            sidebarOpen = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        PromptService.StateChanged -= StateHasChanged;
        
        // Clean up resize listener to prevent memory leaks
        try
        {
            _ = JSRuntime.InvokeVoidAsync("removeResizeListener", DotNetObjectReference.Create(this));
        }
        catch
        {
            // Ignore errors during cleanup
        }
    }
}
