namespace PromptsValut.Services;

public interface ILocalStorageService : IDisposable
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
    Task ClearAsync();
    Task<bool> ContainsKeyAsync(string key);
}
