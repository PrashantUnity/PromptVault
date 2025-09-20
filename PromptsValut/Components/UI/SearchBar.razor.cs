using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PromptsValut.Constants;
using PromptsValut.Services;

namespace PromptsValut.Components.UI;

public partial class SearchBar : ComponentBase, IDisposable
{
    [Parameter] public EventCallback<string> OnSearchChanged { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;

    private string searchQuery = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        searchQuery = PromptService.State.SearchQuery;
        PromptService.StateChanged += StateHasChanged;
        await Task.CompletedTask;
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Search();
        }
    }

    private async Task OnSearchQueryChanged()
    {
        await Search();
    }

    private async Task Search()
    {
        await OnSearchChanged.InvokeAsync(searchQuery);
    }

    public void Dispose()
    {
        PromptService.StateChanged -= StateHasChanged;
    }
}
