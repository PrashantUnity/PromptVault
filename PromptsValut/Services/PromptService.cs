using Microsoft.JSInterop;
using System.Text.Json;
using PromptsValut.Models;

namespace PromptsValut.Services;

public class PromptService : IPromptService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private AppState _state = new();
    private const string EXTERNAL_DATA_URL = "https://raw.githubusercontent.com/codefrydev/Data/refs/heads/main/Prompt/allsources.json";
    private const string EXTERNAL_BASE_PATH = "https://raw.githubusercontent.com/codefrydev/Data/refs/heads/main/Prompt/";

    public event Action? StateChanged;
    public AppState State => _state;
    public bool ShowHelpModal { get; private set; } = false;
    public bool ShowFavoritesModal { get; private set; } = false;
    public List<Prompt> Prompts => _state.Prompts.ToList();
    public List<Category> Categories => _state.Categories.ToList();
    public List<Prompt> FilteredPrompts 
    { 
        get 
        {
            var prompts = _state.Prompts.AsQueryable();

            // Filter by category
            if (_state.SelectedCategory != "all")
            {
                var selected = _state.SelectedCategory;
                prompts = prompts.Where(p => !string.IsNullOrEmpty(p.Category) && p.Category == selected);
            }

            // Filter by search query (null-safe)
            if (!string.IsNullOrEmpty(_state.SearchQuery))
            {
                var query = _state.SearchQuery.ToLower();
                prompts = prompts.Where(p =>
                    (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(p.Content) && p.Content.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(query)) ||
                    (p.Tags != null && p.Tags.Any(t => !string.IsNullOrEmpty(t) && t.ToLower().Contains(query))));
            }

            // Filter by favorites only
            if (_state.ShowFavoritesOnly)
            {
                prompts = prompts.Where(p => _state.Favorites.Contains(p.Id));
            }

            // Sort
            prompts = _state.SortBy switch
            {
                "name" => prompts.OrderBy(p => p.Title),
                "date" => prompts.OrderByDescending(p => p.CreatedAt),
                "rating" => prompts.OrderByDescending(p => p.AverageRating),
                _ => prompts.OrderByDescending(p => p.CreatedAt)
            };

            return prompts.ToList();
        }
    }

    public PromptService(ILocalStorageService localStorage, IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        _state.IsLoading = true;
        NotifyStateChanged();
        
        try
        {
            await LoadStateFromStorageAsync();
            await LoadDefaultDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PromptService: Error during initialization: {ex.Message}");
            // Reset to default state on error
            _state = new AppState();
            await LoadDefaultDataAsync();
        }
        finally
        {
            _state.IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task LoadDataAsync()
    {
        await InitializeAsync();
    }

    public Task<List<Prompt>> GetPromptsAsync()
    {
        return Task.FromResult(_state.Prompts.ToList());
    }

    public Task<List<Category>> GetCategoriesAsync()
    {
        return Task.FromResult(_state.Categories.ToList());
    }


    public Task<Prompt?> GetPromptByIdAsync(string id)
    {
        return Task.FromResult(_state.Prompts.FirstOrDefault(p => p.Id == id));
    }

    public async Task AddPromptAsync(Prompt prompt)
    {
        prompt.Id = Guid.NewGuid().ToString();
        prompt.CreatedAt = DateTime.UtcNow;
        prompt.UpdatedAt = DateTime.UtcNow;
        
        _state.Prompts.Add(prompt);
        UpdateCategoryCounts();
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task UpdatePromptAsync(Prompt prompt)
    {
        var existingPrompt = _state.Prompts.FirstOrDefault(p => p.Id == prompt.Id);
        if (existingPrompt != null)
        {
            var index = _state.Prompts.IndexOf(existingPrompt);
            prompt.UpdatedAt = DateTime.UtcNow;
            _state.Prompts[index] = prompt;
            UpdateCategoryCounts();
            await SaveStateToStorageAsync();
            NotifyStateChanged();
        }
    }

    public async Task DeletePromptAsync(string id)
    {
        var prompt = _state.Prompts.FirstOrDefault(p => p.Id == id);
        if (prompt != null)
        {
            _state.Prompts.Remove(prompt);
            _state.Favorites.Remove(id);
            _state.UserRatings.Remove(id);
            UpdateCategoryCounts();
            await SaveStateToStorageAsync();
            NotifyStateChanged();
        }
    }

    public async Task ToggleFavoriteAsync(string promptId)
    {
        if (_state.Favorites.Contains(promptId))
        {
            _state.Favorites.Remove(promptId);
        }
        else
        {
            _state.Favorites.Add(promptId);
        }
        
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task SetRatingAsync(string promptId, UserRating rating)
    {
        _state.UserRatings[promptId] = rating;
        
        // Update average rating for the prompt
        var prompt = _state.Prompts.FirstOrDefault(p => p.Id == promptId);
        if (prompt != null)
        {
            var ratings = _state.UserRatings.Values.Where(r => r.PromptId == promptId && r.Rating > 0).ToList();
            prompt.AverageRating = ratings.Any() ? ratings.Average(r => r.Rating) : 0;
        }
        
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task SetSearchQueryAsync(string query)
    {
        _state.SearchQuery = query;
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public async Task SetSelectedCategoryAsync(string category)
    {
        _state.SelectedCategory = category;
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(string theme)
    {
        _state.Theme = theme;
        await _jsRuntime.InvokeVoidAsync("setTheme", theme);
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task ToggleThemeAsync()
    {
        _state.Theme = _state.Theme == "light" ? "dark" : "light";
        await _jsRuntime.InvokeVoidAsync("setTheme", _state.Theme);
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task ExportDataAsync()
    {
        var exportData = new ExportData
        {
            Prompts = _state.Prompts,
            Favorites = _state.Favorites,
            UserRatings = _state.UserRatings,
            History = _state.History
        };

        var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
        await _jsRuntime.InvokeVoidAsync("downloadFile", "promptvault-export.json", json);
    }

    public async Task ImportDataAsync(string jsonData)
    {
        try
        {
            var importData = JsonSerializer.Deserialize<ExportData>(jsonData);
            if (importData != null)
            {
                _state.Prompts = importData.Prompts ?? new List<Prompt>();
                _state.Favorites = importData.Favorites ?? new List<string>();
                _state.UserRatings = importData.UserRatings ?? new Dictionary<string, UserRating>();
                _state.History = importData.History ?? new List<string>();
                
                await SaveStateToStorageAsync();
                NotifyStateChanged();
            }
        }
        catch
        {
            // Handle import error
        }
    }

    public async Task ClearDataAsync()
    {
        _state.Prompts.Clear();
        _state.Favorites.Clear();
        _state.UserRatings.Clear();
        _state.History.Clear();
        
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task AddToHistoryAsync(string promptId)
    {
        _state.History.Remove(promptId); // Remove if already exists
        _state.History.Insert(0, promptId); // Add to beginning
        
        // Keep only last 50 items
        if (_state.History.Count > 50)
        {
            _state.History = _state.History.Take(50).ToList();
        }
        
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public Task<List<Prompt>> GetFavoritesAsync()
    {
        return Task.FromResult(_state.Prompts.Where(p => _state.Favorites.Contains(p.Id)).ToList());
    }

    public Task<List<Prompt>> GetHistoryAsync()
    {
        return Task.FromResult(_state.History
            .Select(id => _state.Prompts.FirstOrDefault(p => p.Id == id))
            .Where(p => p != null)
            .Cast<Prompt>()
            .ToList());
    }

    public async Task ShowHelpModalAsync()
    {
        ShowHelpModal = true;
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public async Task HideHelpModalAsync()
    {
        ShowHelpModal = false;
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public async Task ShowFavoritesModalAsync()
    {
        ShowFavoritesModal = true;
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public async Task HideFavoritesModalAsync()
    {
        ShowFavoritesModal = false;
        NotifyStateChanged();
        await Task.CompletedTask;
    }

    public async Task ResetToDefaultStateAsync()
    {
        try
        {
            _state = new AppState();
            await SaveStateToStorageAsync();
            await LoadDefaultDataAsync();
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PromptService: Error resetting to default state: {ex.Message}");
        }
    }

    public async Task<bool> ValidateAndRepairStateAsync()
    {
        try
        {
            var isValid = IsValidAppState(_state);
            if (!isValid)
            {
                Console.WriteLine("PromptService: Invalid state detected, repairing...");
                _state = new AppState();
                await SaveStateToStorageAsync();
                await LoadDefaultDataAsync();
                NotifyStateChanged();
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PromptService: Error validating state: {ex.Message}");
            return false;
        }
    }

    private async Task LoadStateFromStorageAsync()
    {
        try
        {
            var savedState = await _localStorage.GetItemAsync<AppState>("promptvault-state");
            if (savedState != null)
            {
                // Validate the loaded state
                if (IsValidAppState(savedState))
                {
                    _state = savedState;
                }
                else
                {
                    Console.WriteLine("LocalStorageService: Invalid state data found, using default state");
                    _state = new AppState();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error loading state from storage: {ex.Message}");
            _state = new AppState();
        }
    }

    private async Task SaveStateToStorageAsync()
    {
        try
        {
            await _localStorage.SetItemAsync("promptvault-state", _state);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error saving state to storage: {ex.Message}");
            // Could show a toast notification to the user here
        }
    }

    private bool IsValidAppState(AppState state)
    {
        if (state == null) return false;
        
        // Basic validation
        if (state.Prompts == null) state.Prompts = new List<Prompt>();
        if (state.Categories == null) state.Categories = new List<Category>();
        if (state.Favorites == null) state.Favorites = new List<string>();
        if (state.UserRatings == null) state.UserRatings = new Dictionary<string, UserRating>();
        if (state.History == null) state.History = new List<string>();
        
        return true;
    }

    private async Task LoadDefaultDataAsync()
    {
        // Always load categories from external source
        if (!_state.Categories.Any())
        {
            await LoadExternalCategoriesAsync();
        }

        // Then load prompts strictly from category file paths (no fallback)
        if (!_state.Prompts.Any())
        {
            await LoadPromptsFromCategoryFilesAsync();
        }
    }

    private async Task<bool> LoadExternalDataAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(EXTERNAL_DATA_URL);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                
                // Try to parse as different possible structures
                var externalPrompts = await ParseExternalDataAsync(jsonContent);

                if (externalPrompts != null && externalPrompts.Any())
                {
                    // Ensure all prompts have required fields
                    foreach (var prompt in externalPrompts)
                    {
                        if (string.IsNullOrEmpty(prompt.Id))
                            prompt.Id = Guid.NewGuid().ToString();
                        if (prompt.CreatedAt == default)
                            prompt.CreatedAt = DateTime.UtcNow;
                        if (prompt.UpdatedAt == default)
                            prompt.UpdatedAt = DateTime.UtcNow;
                        if (string.IsNullOrEmpty(prompt.Category))
                            prompt.Category = "general";
                        if (string.IsNullOrEmpty(prompt.Author))
                            prompt.Author = "External Source";
                    }

                    _state.Prompts.AddRange(externalPrompts);
                    UpdateCategoryCounts();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception if needed, but don't throw
            // This allows the app to continue with local data
            Console.WriteLine($"Failed to load external data: {ex.Message}");
        }

        return false;
    }

    private Task<List<Prompt>?> ParseExternalDataAsync(string jsonContent)
    {
        try
        {
            // Try to parse as direct array of prompts
            var prompts = JsonSerializer.Deserialize<List<Prompt>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (prompts != null && prompts.Any())
                return Task.FromResult<List<Prompt>?>(prompts);
        }
        catch { }

        try
        {
            // Try to parse as object with prompts property
            using var document = JsonDocument.Parse(jsonContent);
            if (document.RootElement.TryGetProperty("prompts", out var promptsElement))
            {
                var prompts = JsonSerializer.Deserialize<List<Prompt>>(promptsElement.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (prompts != null && prompts.Any())
                    return Task.FromResult<List<Prompt>?>(prompts);
            }
        }
        catch { }

        try
        {
            // Try to parse as object with data property
            using var document = JsonDocument.Parse(jsonContent);
            if (document.RootElement.TryGetProperty("data", out var dataElement))
            {
                var prompts = JsonSerializer.Deserialize<List<Prompt>>(dataElement.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (prompts != null && prompts.Any())
                    return Task.FromResult<List<Prompt>?>(prompts);
            }
        }
        catch { }

        try
        {
            // Try to parse as dictionary/object where values are prompts
            using var document = JsonDocument.Parse(jsonContent);
            var prompts = new List<Prompt>();
            
            foreach (var property in document.RootElement.EnumerateObject())
            {
                try
                {
                    var prompt = JsonSerializer.Deserialize<Prompt>(property.Value.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (prompt != null)
                    {
                        // If the prompt doesn't have a title, use the property name
                        if (string.IsNullOrEmpty(prompt.Title))
                            prompt.Title = property.Name;
                        
                        prompts.Add(prompt);
                    }
                }
                catch { }
            }
            
            if (prompts.Any())
                return Task.FromResult<List<Prompt>?>(prompts);
        }
        catch { }

        return Task.FromResult<List<Prompt>?>(null);
    }

    private void UpdateCategoryCounts()
    {
            foreach (var category in _state.Categories)
        {
            if (category.Id == "all")
            {
                category.PromptCount = _state.Prompts.Count;
            }
            else
            {
                    category.PromptCount = _state.Prompts.Count(p => !string.IsNullOrEmpty(p.Category) && p.Category == category.Id);
            }
        }
    }

    private async Task<bool> LoadPromptsFromCategoryFilesAsync()
    {
        try
        {
            if (_state.Categories == null || _state.Categories.Count == 0)
            {
                return false;
            }

            var aggregated = new List<Prompt>();

            foreach (var category in _state.Categories)
            {
                // Skip the synthetic "all" category
                if (string.Equals(category.Id, "all", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(category.FilePathName))
                {
                    continue;
                }

                try
                {
                    var response = await _httpClient.GetAsync(category.FilePathName);
                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var prompts = await ParseExternalDataAsync(json) ?? new List<Prompt>();

                    foreach (var prompt in prompts)
                    {
                        if (string.IsNullOrEmpty(prompt.Id))
                            prompt.Id = Guid.NewGuid().ToString();
                        if (prompt.CreatedAt == default)
                            prompt.CreatedAt = DateTime.UtcNow;
                        if (prompt.UpdatedAt == default)
                            prompt.UpdatedAt = DateTime.UtcNow;
                        // Always set the prompt category to the normalized category Id
                        prompt.Category = category.Id;
                        if (string.IsNullOrEmpty(prompt.Author))
                            prompt.Author = "External Source";
                    }

                    aggregated.AddRange(prompts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load prompts for category '{category.Id}': {ex.Message}");
                }
            }

            if (aggregated.Count > 0)
            {
                _state.Prompts.AddRange(aggregated);
                UpdateCategoryCounts();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading prompts from category files: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> LoadExternalCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(EXTERNAL_DATA_URL);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(jsonContent);
                if (document.RootElement.ValueKind == JsonValueKind.Object &&
                    document.RootElement.TryGetProperty("categories", out var categoriesElement) &&
                    categoriesElement.ValueKind == JsonValueKind.Array)
                {
                    var categories = JsonSerializer.Deserialize<List<Category>>(categoriesElement.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Category>();

                    foreach (var category in categories)
                    {
                        // Always normalize Id from Name (slug) for consistent filtering keys
                        if (!string.IsNullOrWhiteSpace(category.Name))
                        {
                            var slug = category.Name.Trim().ToLower();
                            slug = string.Join("-", slug.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                            category.Id = slug;
                        }

                        if (!string.IsNullOrWhiteSpace(category.FilePathName))
                        {
                            if (!category.FilePathName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                                !category.FilePathName.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            {
                                category.FilePathName = EXTERNAL_BASE_PATH + category.FilePathName.TrimStart('/');
                            }
                        }
                    }

                    // Always inject synthetic "all" category for UI filtering
                    if (!categories.Any(c => string.Equals(c.Id, "all", StringComparison.OrdinalIgnoreCase)))
                    {
                        categories.Insert(0, new Category
                        {
                            Id = "all",
                            Name = "All Prompts",
                            Description = "View all available prompts",
                            Icon = "LayoutGrid",
                            Color = "blue",
                            SortOrder = 0,
                            PromptCount = 0
                        });
                    }

                    _state.Categories.Clear();
                    _state.Categories.AddRange(categories.OrderBy(c => c.SortOrder));
                    
                    // Ensure selected category is valid
                    if (!_state.Categories.Any(c => c.Id == _state.SelectedCategory))
                    {
                        _state.SelectedCategory = "all";
                    }
                    UpdateCategoryCounts();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load external categories: {ex.Message}");
        }

        return false;
    }

    // No local fallback categories; categories must come from external source

    public async Task SetShowFavoritesOnlyAsync(bool showFavoritesOnly)
    {
        _state.ShowFavoritesOnly = showFavoritesOnly;
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task RefreshExternalDataAsync()
    {
        _state.IsLoading = true;
        NotifyStateChanged();
        
        try
        {
            // Clear existing prompts (but keep user data like favorites and ratings)
            var existingFavorites = _state.Favorites.ToList();
            var existingRatings = _state.UserRatings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            _state.Prompts.Clear();
            _state.Categories.Clear();

            // Load categories strictly from external
            await LoadExternalCategoriesAsync();

            // Then load prompts strictly from category files
            await LoadPromptsFromCategoryFilesAsync();
            
            // Restore user data
            _state.Favorites = existingFavorites;
            _state.UserRatings = existingRatings;
            
            await SaveStateToStorageAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PromptService: Error during refresh: {ex.Message}");
        }
        finally
        {
            _state.IsLoading = false;
            NotifyStateChanged();
        }
    }


    private void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
