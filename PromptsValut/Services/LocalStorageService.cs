using Microsoft.JSInterop;
using System.Text.Json;

namespace PromptsValut.Services;

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("LocalStorageService: GetItemAsync called with null or empty key");
                return default;
            }

            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"LocalStorageService: JSON deserialization error for key '{key}': {ex.Message}");
            // Try to clean up corrupted data
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error getting item '{key}': {ex.Message}");
            return default;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("LocalStorageService: SetItemAsync called with null or empty key");
                return;
            }

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"LocalStorageService: JSON serialization error for key '{key}': {ex.Message}");
            throw new InvalidOperationException($"Failed to serialize data for key '{key}'", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error setting item '{key}': {ex.Message}");
            throw new InvalidOperationException($"Failed to save data for key '{key}'", ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("LocalStorageService: RemoveItemAsync called with null or empty key");
                return;
            }

            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error removing item '{key}': {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error clearing storage: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> ContainsKeyAsync(string key)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var result = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            return !string.IsNullOrEmpty(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorageService: Error checking key '{key}': {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}
