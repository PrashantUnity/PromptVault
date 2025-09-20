using Microsoft.AspNetCore.Components;
using PromptsValut.Constants;

namespace PromptsValut.Components.Modals;

public partial class HelpModal : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; } = false;
    [Parameter] public EventCallback OnClose { get; set; }

    private int activeTab = 0;

    private void SetActiveTab(int tabIndex)
    {
        activeTab = tabIndex;
        StateHasChanged();
    }

    private async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }
}
