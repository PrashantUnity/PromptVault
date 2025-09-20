using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using PromptsValut.Constants;
using PromptsValut.Models;
using PromptsValut.Services;

namespace PromptsValut.Components.Modals;

public partial class ExportImportModal : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; } = false;
    [Parameter] public EventCallback OnClose { get; set; }

    [Inject] private IPromptService PromptService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference fileInput;
    private string selectedFileName = "";
    private string importPreview = "";
    private ExportData? importPreviewData;
    private bool showClearConfirmation = false;

    private async Task CloseModal()
    {
        await OnClose.InvokeAsync();
    }

    private async Task ExportData()
    {
        await PromptService.ExportDataAsync();
    }

    private async Task SelectFile()
    {
        await JSRuntime.InvokeVoidAsync("clickFileInput", fileInput);
    }

    private async Task OnFileSelected(ChangeEventArgs e)
    {
        var files = (IBrowserFile[]?)e.Value;
        if (files?.Length > 0)
        {
            var file = files[0];
            selectedFileName = file.Name;
            
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            importPreview = await reader.ReadToEndAsync();
            
            try
            {
                importPreviewData = System.Text.Json.JsonSerializer.Deserialize<ExportData>(importPreview);
            }
            catch
            {
                importPreview = "";
                importPreviewData = null;
                selectedFileName = "Invalid JSON file";
            }
            
            StateHasChanged();
        }
    }

    private async Task ImportData()
    {
        if (!string.IsNullOrEmpty(importPreview) && importPreviewData != null)
        {
            await PromptService.ImportDataAsync(importPreview);
            await CloseModal();
        }
    }

    private void CancelImport()
    {
        selectedFileName = "";
        importPreview = "";
        importPreviewData = null;
    }

    private void ShowClearConfirmation()
    {
        showClearConfirmation = true;
    }

    private void HideClearConfirmation()
    {
        showClearConfirmation = false;
    }

    private async Task ClearAllData()
    {
        await PromptService.ClearDataAsync();
        showClearConfirmation = false;
        await CloseModal();
    }
}
