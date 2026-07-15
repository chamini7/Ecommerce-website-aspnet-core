using Blazored.LocalStorage;

namespace ECommerceBlazor.Services;

// A simple, beginner-friendly way to track "is the user logged in" across pages.
// (In a bigger app you'd use Blazor's AuthenticationStateProvider - mention this in interviews
// as a possible improvement, it shows you know the "proper" way too.)
public class AuthStateService
{
    private readonly ILocalStorageService _localStorage;

    public string? Token { get; private set; }
    public string? FullName { get; private set; }
    public string? Role { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);
    public bool IsAdmin => Role == "Admin";

    public event Action? OnChange;

    public AuthStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        Token = await _localStorage.GetItemAsStringAsync("authToken");
        FullName = await _localStorage.GetItemAsStringAsync("fullName");
        Role = await _localStorage.GetItemAsStringAsync("role");
        OnChange?.Invoke();
    }

    public async Task LoginAsync(string token, string fullName, string role)
    {
        await _localStorage.SetItemAsStringAsync("authToken", token);
        await _localStorage.SetItemAsStringAsync("fullName", fullName);
        await _localStorage.SetItemAsStringAsync("role", role);
        Token = token;
        FullName = fullName;
        Role = role;
        OnChange?.Invoke();
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("fullName");
        await _localStorage.RemoveItemAsync("role");
        Token = null;
        FullName = null;
        Role = null;
        OnChange?.Invoke();
    }
}
