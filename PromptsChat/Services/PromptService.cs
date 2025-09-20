using Microsoft.JSInterop;
using PromptsChat.Models;
using System.Text.Json;

namespace PromptsChat.Services;

public class PromptService : IPromptService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private AppState _state = new();

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
                    p.Title.ToLower().Contains(query) ||
                    p.Content.ToLower().Contains(query) ||
                    p.Description.ToLower().Contains(query) ||
                    p.Tags.Any(t => t.ToLower().Contains(query)));
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

    public PromptService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
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

    public Task<List<Prompt>> GetFilteredPromptsAsync()
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
                p.Title.ToLower().Contains(query) ||
                p.Description.ToLower().Contains(query) ||
                p.Content.ToLower().Contains(query) ||
                p.Tags.Any(tag => tag.ToLower().Contains(query)));
        }

        // Filter by favorites only
        if (_state.ShowFavoritesOnly)
        {
            prompts = prompts.Where(p => _state.Favorites.Contains(p.Id));
        }

        // Sort
        prompts = _state.SortBy switch
        {
            "newest" => prompts.OrderByDescending(p => p.CreatedAt),
            "oldest" => prompts.OrderBy(p => p.CreatedAt),
            "title" => prompts.OrderBy(p => p.Title),
            "rating" => prompts.OrderByDescending(p => p.AverageRating),
            "usage" => prompts.OrderByDescending(p => p.UsageCount),
            _ => prompts.OrderByDescending(p => p.CreatedAt)
        };

        return Task.FromResult(prompts.ToList());
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
    }

    public async Task SetSelectedCategoryAsync(string category)
    {
        _state.SelectedCategory = category;
        NotifyStateChanged();
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
    }

    public async Task HideHelpModalAsync()
    {
        ShowHelpModal = false;
        NotifyStateChanged();
    }

    private async Task LoadStateFromStorageAsync()
    {
        var savedState = await _localStorage.GetItemAsync<AppState>("promptvault-state");
        if (savedState != null)
        {
            _state = savedState;
        }
    }

    private async Task SaveStateToStorageAsync()
    {
        await _localStorage.SetItemAsync("promptvault-state", _state);
    }

    private async Task LoadDefaultDataAsync()
    {
        if (!_state.Prompts.Any())
        {
            await LoadDefaultPromptsAsync();
        }
        
        if (!_state.Categories.Any())
        {
            await LoadDefaultCategoriesAsync();
        }
    }

    private async Task LoadDefaultPromptsAsync()
    {
        var defaultPrompts = new List<Prompt>
        {
            new Prompt
            {
                Id = "1",
                Title = "Product Launch Email Campaign",
                Content = "Create a comprehensive email marketing campaign for a new product launch. Include:\n\n1. A series of 5 emails (teaser, announcement, features, testimonials, final call)\n2. Subject lines optimized for open rates\n3. Compelling CTAs for each email\n4. Personalization strategies\n5. A/B testing suggestions\n\nTarget audience: [Define your audience]\nProduct: [Describe your product]\nLaunch date: [Specify date]\nKey selling points: [List main benefits]",
                Category = "marketing",
                Description = "Create comprehensive email marketing campaigns for product launches",
                Tags = new[] { "email", "campaign", "product launch" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                UsageNotes = "Replace bracketed placeholders with your specific information. Consider your brand voice when implementing.",
                EstimatedTime = "15-30 minutes",
                CreatedAt = new DateTime(2024, 1, 15),
                UsageCount = 45,
                AverageRating = 4.2
            },
            new Prompt
            {
                Id = "2",
                Title = "React Component Generator",
                Content = "Generate a fully functional React component with TypeScript that includes:\n\nComponent name: [ComponentName]\nProps: [List required props with types]\nFeatures:\n- Proper TypeScript typing\n- Responsive design with Tailwind CSS\n- Accessibility features (ARIA labels, keyboard navigation)\n- Error handling\n- Loading states\n- Proper documentation with JSDoc comments\n\nInclude example usage and unit test suggestions.",
                Category = "development",
                Description = "Generate complete React components with TypeScript, Tailwind CSS, and best practices",
                Tags = new[] { "react", "typescript", "component" },
                Author = "PromptVault Team",
                Difficulty = "advanced",
                UsageNotes = "Customize the component requirements based on your specific needs.",
                CreatedAt = new DateTime(2024, 1, 10),
                UsageCount = 78,
                AverageRating = 4.5
            },
            new Prompt
            {
                Id = "3",
                Title = "Creative Story Outline",
                Content = "Create a detailed story outline with the following elements:\n\nGenre: [Specify genre]\nSetting: [Time period and location]\nMain character: [Brief description]\nCentral conflict: [Core problem/challenge]\n\nInclude:\n1. Three-act structure breakdown\n2. Character arc progression\n3. 5 major plot points\n4. 3 subplots that interconnect\n5. Themes to explore\n6. Potential opening and closing scenes\n7. Unique twist or hook that sets it apart",
                Category = "creative-writing",
                Description = "Generate detailed story outlines with three-act structure and character development",
                Tags = new[] { "fiction", "storytelling", "outline" },
                Author = "PromptVault Team",
                Difficulty = "beginner",
                EstimatedTime = "10-20 minutes",
                CreatedAt = new DateTime(2024, 1, 8),
                UsageCount = 32,
                AverageRating = 4.0
            },
            new Prompt
            {
                Id = "4",
                Title = "Business Strategy Analysis",
                Content = "Conduct a comprehensive business strategy analysis for [Company/Industry]:\n\n1. SWOT Analysis\n  - Strengths (internal)\n  - Weaknesses (internal)\n  - Opportunities (external)\n  - Threats (external)\n\n2. Porter's Five Forces evaluation\n3. Competitive landscape assessment\n4. Market trend analysis\n5. Growth opportunities identification\n6. Risk mitigation strategies\n7. 3-year strategic roadmap with milestones\n8. Key performance indicators (KPIs) to track\n\nProvide actionable recommendations with priority levels.",
                Category = "business",
                Description = "Conduct comprehensive business strategy analysis with SWOT and Porter's Five Forces",
                Tags = new[] { "strategy", "analysis", "planning" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                CreatedAt = new DateTime(2024, 1, 5),
                UsageCount = 23,
                AverageRating = 4.3
            },
            new Prompt
            {
                Id = "5",
                Title = "Interactive Learning Module",
                Content = "Design an interactive learning module for teaching [Subject/Topic]:\n\nTarget audience: [Age group/Level]\nDuration: [Time frame]\nLearning objectives: [List 3-5 objectives]\n\nInclude:\n1. Pre-assessment questions\n2. Content broken into digestible sections\n3. Interactive elements (quizzes, simulations, discussions)\n4. Real-world application examples\n5. Visual aids and multimedia suggestions\n6. Practice exercises with varying difficulty\n7. Assessment rubric\n8. Additional resources for further learning\n\nMake it engaging and accommodate different learning styles.",
                Category = "education",
                Description = "Design interactive learning modules with assessments and multimedia elements",
                Tags = new[] { "teaching", "curriculum", "interactive" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                CreatedAt = new DateTime(2024, 1, 3),
                UsageCount = 19,
                AverageRating = 4.1
            },
            new Prompt
            {
                Id = "6",
                Title = "API Documentation Template",
                Content = "Create comprehensive API documentation for [API Name]:\n\nBase URL: [Your API URL]\nAuthentication: [Method used]\n\nFor each endpoint, document:\n1. HTTP method and path\n2. Description and use case\n3. Request parameters (query, path, body)\n4. Request/response examples\n5. Status codes and error handling\n6. Rate limiting information\n7. Versioning details\n8. Code examples in multiple languages (curl, JavaScript, Python)\n\nInclude getting started guide and best practices section.",
                Category = "technology",
                Description = "Generate detailed API documentation with examples and best practices",
                Tags = new[] { "API", "documentation", "technical" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                CreatedAt = new DateTime(2024, 1, 1),
                UsageCount = 67,
                AverageRating = 4.4
            },
            new Prompt
            {
                Id = "7",
                Title = "Comedy Sketch Generator",
                Content = "Write a comedy sketch with the following parameters:\n\nSetting: [Location/Situation]\nCharacters: [2-4 character types]\nDuration: [3-5 minutes when performed]\nComedy style: [Observational/Absurdist/Satirical/Physical]\n\nInclude:\n1. Strong opening hook\n2. Escalating absurdity or conflict\n3. Callback jokes to earlier setup\n4. Physical comedy opportunities\n5. Unexpected twist or reversal\n6. Punchy ending/punchline\n7. Stage directions for timing\n\nMake it relatable while pushing boundaries of the absurd.",
                Category = "fun",
                Description = "Generate comedy sketches with various styles and timing",
                Tags = new[] { "comedy", "entertainment", "creative" },
                Author = "PromptVault Team",
                Difficulty = "beginner",
                CreatedAt = new DateTime(2023, 12, 28),
                UsageCount = 89,
                AverageRating = 4.6
            },
            new Prompt
            {
                Id = "8",
                Title = "Personal Productivity System",
                Content = "Design a personalized productivity system based on:\n\nWork style: [Deep work/Collaborative/Hybrid]\nMain challenges: [List your productivity blockers]\nTools available: [Digital/Analog/Both]\nTime constraints: [Daily schedule overview]\n\nCreate:\n1. Morning routine optimization\n2. Task prioritization framework\n3. Time-blocking schedule template\n4. Focus session structure\n5. Break and recovery protocols\n6. Weekly review process\n7. Goal-setting methodology\n8. Habit tracking system\n9. Distraction elimination strategies\n\nInclude specific tools and apps recommendations.",
                Category = "productivity",
                Description = "Create personalized productivity systems with tools and strategies",
                Tags = new[] { "efficiency", "time-management", "organization" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                CreatedAt = new DateTime(2023, 12, 25),
                UsageCount = 56,
                AverageRating = 4.2
            },
            new Prompt
            {
                Id = "9",
                Title = "Data Visualization Dashboard",
                Content = "Design a data visualization dashboard for [Dataset/Business Area]:\n\nData sources: [List available data]\nKey metrics: [What needs tracking]\nUpdate frequency: [Real-time/Daily/Weekly]\nUsers: [Who will use this]\n\nInclude:\n1. Dashboard layout and information hierarchy\n2. Chart types for each metric (bar, line, pie, etc.)\n3. Color scheme and visual design principles\n4. Interactive filtering options\n5. Drill-down capabilities\n6. Export and sharing features\n7. Mobile responsiveness considerations\n8. Performance optimization strategies\n9. Alert and threshold configurations\n\nProvide implementation suggestions using modern BI tools.",
                Category = "data-analysis",
                Description = "Design comprehensive data visualization dashboards with modern BI tools",
                Tags = new[] { "visualization", "dashboard", "analytics" },
                Author = "PromptVault Team",
                Difficulty = "advanced",
                CreatedAt = new DateTime(2023, 12, 22),
                UsageCount = 34,
                AverageRating = 4.3
            },
            new Prompt
            {
                Id = "10",
                Title = "Social Media Content Calendar",
                Content = "Create a 30-day social media content calendar for [Brand/Purpose]:\n\nPlatforms: [List your platforms]\nPosting frequency: [Daily/Weekly per platform]\nBrand voice: [Describe tone and style]\nGoals: [Engagement/Sales/Awareness]\n\nFor each post include:\n1. Platform-specific content format\n2. Caption with hashtags\n3. Visual content description\n4. Best posting time\n5. Call-to-action\n6. Engagement strategy\n7. Cross-platform synergy\n\nMix content types: Educational, entertaining, promotional, user-generated, behind-the-scenes.",
                Category = "marketing",
                Description = "Create comprehensive social media content calendars with engagement strategies",
                Tags = new[] { "social media", "content", "planning" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                CreatedAt = new DateTime(2023, 12, 20),
                UsageCount = 42,
                AverageRating = 4.1
            },
            new Prompt
            {
                Id = "11",
                Title = "Website Design Brief",
                Content = "Create a comprehensive website design brief for [Company Name]:\n\nBusiness Type: [Select business type]\nTarget Audience: [Describe your target audience]\nWebsite Purpose: [What should the website accomplish]\nBrand Colors: [Primary and secondary colors]\nTone: [Professional/Casual/Creative/Technical]\nKey Pages: [List main pages needed]\nBudget Range: [Select budget range]\nTimeline: [Project duration]\n\nInclude:\n1. User experience goals\n2. Visual design preferences\n3. Content requirements\n4. Technical specifications\n5. Success metrics\n6. Competitor analysis\n7. Mobile responsiveness needs\n8. Accessibility requirements",
                Category = "development",
                Description = "Create comprehensive website design briefs with technical specifications",
                Tags = new[] { "web design", "brief", "planning" },
                Author = "PromptVault Team",
                Difficulty = "intermediate",
                UsageNotes = "Use the form to customize this brief for your specific project needs.",
                CreatedAt = new DateTime(2023, 12, 18),
                UsageCount = 28,
                AverageRating = 4.0
            },
            new Prompt
            {
                Id = "12",
                Title = "Email Newsletter Template",
                Content = "Design an email newsletter template for [Newsletter Name]:\n\nIndustry: [Select your industry]\nAudience Size: [Number of subscribers]\nFrequency: [How often you send]\nContent Focus: [What type of content]\nBrand Voice: [Describe your tone]\nCall-to-Action: [What action should readers take]\n\nInclude:\n1. Header design with logo placement\n2. Navigation menu\n3. Featured article section\n4. Secondary content blocks\n5. Social media links\n6. Footer with contact info\n7. Mobile optimization\n8. A/B testing suggestions\n9. Subject line recommendations\n10. Unsubscribe compliance",
                Category = "marketing",
                Description = "Design professional email newsletter templates with mobile optimization",
                Tags = new[] { "email", "newsletter", "template" },
                Author = "PromptVault Team",
                Difficulty = "beginner",
                UsageNotes = "Customize the template based on your brand and audience preferences.",
                CreatedAt = new DateTime(2023, 12, 15),
                UsageCount = 51,
                AverageRating = 4.4
            }
        };

        _state.Prompts.AddRange(defaultPrompts);
    }

    private Task LoadDefaultCategoriesAsync()
    {
        var defaultCategories = new List<Category>
        {
            new Category { Id = "all", Name = "All Prompts", Description = "View all available prompts", Icon = "LayoutGrid", Color = "blue", SortOrder = 0, PromptCount = 12 },
            new Category { Id = "marketing", Name = "Marketing", Description = "Marketing and promotional content", Icon = "TrendingUp", Color = "pink", SortOrder = 1, PromptCount = 3 },
            new Category { Id = "development", Name = "Development", Description = "Code generation and development tools", Icon = "Code2", Color = "green", SortOrder = 2, PromptCount = 2 },
            new Category { Id = "creative-writing", Name = "Creative Writing", Description = "Creative writing and storytelling", Icon = "PenTool", Color = "purple", SortOrder = 3, PromptCount = 1 },
            new Category { Id = "business", Name = "Business", Description = "Business strategy and analysis", Icon = "Briefcase", Color = "blue", SortOrder = 4, PromptCount = 1 },
            new Category { Id = "education", Name = "Education", Description = "Educational content and learning", Icon = "GraduationCap", Color = "green", SortOrder = 5, PromptCount = 1 },
            new Category { Id = "technology", Name = "Technology", Description = "Technology and technical documentation", Icon = "Cpu", Color = "orange", SortOrder = 6, PromptCount = 1 },
            new Category { Id = "fun", Name = "Fun", Description = "Entertainment and creative content", Icon = "Sparkles", Color = "yellow", SortOrder = 7, PromptCount = 1 },
            new Category { Id = "productivity", Name = "Productivity", Description = "Productivity and efficiency tools", Icon = "Zap", Color = "purple", SortOrder = 8, PromptCount = 1 },
            new Category { Id = "data-analysis", Name = "Data Analysis", Description = "Data visualization and analytics", Icon = "BarChart3", Color = "blue", SortOrder = 9, PromptCount = 1 }
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

    public async Task SetSortByAsync(string sortBy)
    {
        _state.SortBy = sortBy;
        await SaveStateToStorageAsync();
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
