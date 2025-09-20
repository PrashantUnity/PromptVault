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
    private const string EXTERNAL_DATA_URL = "https://raw.githubusercontent.com/codefrydev/Data/refs/heads/main/Prompt/data.json";

    public event Action? StateChanged;
    public AppState State => _state;
    public bool ShowHelpModal { get; private set; } = false;
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
                prompts = prompts.Where(p => p.Category == _state.SelectedCategory);
            }

            // Filter by search query
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
        await LoadStateFromStorageAsync();
        await LoadDefaultDataAsync();
        NotifyStateChanged();
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
        if (!_state.Prompts.Any())
        {
            // Always load from external source
            await LoadExternalDataAsync();
        }
        
        if (!_state.Categories.Any())
        {
            await LoadDefaultCategoriesAsync();
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
                category.PromptCount = _state.Prompts.Count(p => p.Category == category.Id);
            }
        }
    }

    private Task LoadDefaultCategoriesAsync()
    {
        var defaultCategories = new List<Category>
        {
            new Category { Id = "all", Name = "All Prompts", Description = "View all available prompts", Icon = "LayoutGrid", Color = "blue", SortOrder = 0, PromptCount = 0 },
            new Category { Id = "marketing", Name = "Marketing", Description = "Marketing and promotional content", Icon = "TrendingUp", Color = "pink", SortOrder = 1, PromptCount = 0 },
            new Category { Id = "development", Name = "Development", Description = "Code generation and development tools", Icon = "Code2", Color = "green", SortOrder = 2, PromptCount = 0 },
            new Category { Id = "creative-writing", Name = "Creative Writing", Description = "Creative writing and storytelling", Icon = "PenTool", Color = "purple", SortOrder = 3, PromptCount = 0 },
            new Category { Id = "business", Name = "Business", Description = "Business strategy and analysis", Icon = "Briefcase", Color = "blue", SortOrder = 4, PromptCount = 0 },
            new Category { Id = "education", Name = "Education", Description = "Educational content and learning", Icon = "GraduationCap", Color = "green", SortOrder = 5, PromptCount = 0 },
            new Category { Id = "technology", Name = "Technology", Description = "Technology and technical documentation", Icon = "Cpu", Color = "orange", SortOrder = 6, PromptCount = 0 },
            new Category { Id = "fun", Name = "Fun", Description = "Entertainment and creative content", Icon = "Sparkles", Color = "yellow", SortOrder = 7, PromptCount = 0 },
            new Category { Id = "productivity", Name = "Productivity", Description = "Productivity and efficiency tools", Icon = "Zap", Color = "purple", SortOrder = 8, PromptCount = 0 },
            new Category { Id = "data-analysis", Name = "Data Analysis", Description = "Data visualization and analytics", Icon = "BarChart3", Color = "blue", SortOrder = 9, PromptCount = 0 },
            new Category { Id = "testing", Name = "Testing", Description = "Test prompts and examples", Icon = "CheckCircle", Color = "gray", SortOrder = 10, PromptCount = 0 },
            new Category { Id = "general", Name = "General", Description = "General purpose prompts", Icon = "FileText", Color = "gray", SortOrder = 11, PromptCount = 0 }
        };

        _state.Categories.AddRange(defaultCategories);
        return Task.CompletedTask;
    }

    public async Task SetShowFavoritesOnlyAsync(bool showFavoritesOnly)
    {
        _state.ShowFavoritesOnly = showFavoritesOnly;
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    public async Task RefreshExternalDataAsync()
    {
        // Clear existing prompts (but keep user data like favorites and ratings)
        var existingFavorites = _state.Favorites.ToList();
        var existingRatings = _state.UserRatings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        _state.Prompts.Clear();
        
        // Always load from external source
        await LoadExternalDataAsync();
        
        // Restore user data
        _state.Favorites = existingFavorites;
        _state.UserRatings = existingRatings;
        
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }


    private void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
