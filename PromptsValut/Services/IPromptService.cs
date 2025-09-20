using PromptsValut.Models;

namespace PromptsValut.Services;

public interface IPromptService
{
    event Action? StateChanged;
    AppState State { get; }
    bool ShowHelpModal { get; }
    List<Prompt> Prompts { get; }
    List<Category> Categories { get; }
    List<Prompt> FilteredPrompts { get; }
    
    Task InitializeAsync();
    Task LoadDataAsync();
    Task<List<Prompt>> GetPromptsAsync();
    Task<List<Category>> GetCategoriesAsync();
    Task<Prompt?> GetPromptByIdAsync(string id);
    Task AddPromptAsync(Prompt prompt);
    Task UpdatePromptAsync(Prompt prompt);
    Task DeletePromptAsync(string id);
    Task ToggleFavoriteAsync(string promptId);
    Task SetRatingAsync(string promptId, UserRating rating);
    Task SetSearchQueryAsync(string query);
    Task SetSelectedCategoryAsync(string category);
    Task SetShowFavoritesOnlyAsync(bool showFavoritesOnly);
    Task SetThemeAsync(string theme);
    Task ToggleThemeAsync();
    Task ExportDataAsync();
    Task ImportDataAsync(string jsonData);
    Task ClearDataAsync();
    Task AddToHistoryAsync(string promptId);
    Task<List<Prompt>> GetFavoritesAsync();
    Task<List<Prompt>> GetHistoryAsync();
    Task ShowHelpModalAsync();
    Task HideHelpModalAsync();
    Task RefreshExternalDataAsync();
    Task ResetToDefaultStateAsync();
    Task<bool> ValidateAndRepairStateAsync();
}
