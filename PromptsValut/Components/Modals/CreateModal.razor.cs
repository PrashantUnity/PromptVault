using Microsoft.AspNetCore.Components;
using PromptsValut.Constants;
using PromptsValut.Models;
using PromptsValut.Services;

namespace PromptsValut.Components.Modals;

public partial class CreateModal : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; } = false;
    [Parameter] public EventCallback OnClose { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;

    private Prompt newPrompt = new();
    private string tagsInput = "";

    protected override void OnParametersSet()
    {
        if (IsVisible)
        {
            ResetForm();
        }
    }

    private void ResetForm()
    {
        newPrompt = new Prompt
        {
            Author = "You",
            IsPublic = true,
            Difficulty = "beginner"
        };
        tagsInput = "";
    }

    private async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }

    private async Task CreatePrompt()
    {
        if (string.IsNullOrWhiteSpace(newPrompt.Title) || string.IsNullOrWhiteSpace(newPrompt.Content) || string.IsNullOrWhiteSpace(newPrompt.Category))
        {
            return;
        }

        // Parse tags
        if (!string.IsNullOrWhiteSpace(tagsInput))
        {
            newPrompt.Tags = tagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToArray();
        }
        else
        {
            newPrompt.Tags = Array.Empty<string>();
        }

        await PromptService.AddPromptAsync(newPrompt);
        await CloseModal();
    }
}
