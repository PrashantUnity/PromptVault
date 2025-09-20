using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PromptsValut.Components.UI;

public partial class LocalStorageDebug : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private string debugMessage = string.Empty;

    private async Task DebugLocalStorage()
    {
        try
        {
            var keys = await JSRuntime.InvokeAsync<string[]>("debugLocalStorage");
            debugMessage = $"Found {keys.Length} items in localStorage. Check console for details.";
        }
        catch (Exception ex)
        {
            debugMessage = $"Error: {ex.Message}";
        }
    }

    private async Task ClearLocalStorage()
    {
        try
        {
            var result = await JSRuntime.InvokeAsync<bool>("clearLocalStorage");
            debugMessage = result ? "Local storage cleared successfully" : "Failed to clear local storage";
        }
        catch (Exception ex)
        {
            debugMessage = $"Error: {ex.Message}";
        }
    }

    private async Task CheckAvailability()
    {
        try
        {
            var available = await JSRuntime.InvokeAsync<bool>("isLocalStorageAvailable");
            debugMessage = available ? "Local storage is available" : "Local storage is not available";
        }
        catch (Exception ex)
        {
            debugMessage = $"Error: {ex.Message}";
        }
    }

    private void ToggleVisibility()
    {
        // This would typically be handled by a parent component
        // For now, we'll just clear the debug message
        debugMessage = string.Empty;
    }
}
