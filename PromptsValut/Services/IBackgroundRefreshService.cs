namespace PromptsValut.Services;

public interface IBackgroundRefreshService : IDisposable
{
    Task StartAsync();
    Task StopAsync();
    Task RefreshDataAsync();
    bool IsRunning { get; }
    DateTime LastRefresh { get; }
    int RefreshIntervalMinutes { get; set; }
    bool IsEnabled { get; set; }
}
