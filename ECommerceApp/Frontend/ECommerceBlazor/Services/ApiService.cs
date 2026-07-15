using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace ECommerceBlazor.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public ApiService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    // Attaches the saved JWT token to every request (like an axios interceptor in React)
    private async Task AttachTokenAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync("authToken");
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token.Trim('"'));
    }

    public record ProductDto(int Id, string Name, string Description, decimal Price, int StockQuantity, string Category, string? ImageUrl);
    public record AuthResponse(string Token, string Email, string FullName, string Role);
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string FullName, string Email, string Password);
    public record AddToCartRequest(int ProductId, int Quantity);
    public record ProductWriteRequest(string Name, string Description, decimal Price, int StockQuantity, string Category, string ImageUrl);

    public async Task<List<ProductDto>?> GetProductsAsync(string? search = null)
    {
        var url = "api/products" + (string.IsNullOrEmpty(search) ? "" : $"?search={search}");
        var result = await _http.GetFromJsonAsync<ProductsResponse>(url);
        return result?.Products;
    }

    public record ProductsResponse(int TotalCount, int Page, int PageSize, List<ProductDto> Products);

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest(email, password));
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<AuthResponse?> RegisterAsync(string fullName, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", new RegisterRequest(fullName, email, password));
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<HttpResponseMessage> AddToCartAsync(int productId, int quantity)
    {
        await AttachTokenAsync();
        return await _http.PostAsJsonAsync("api/cart/items", new AddToCartRequest(productId, quantity));
    }

    public record CartItemDto(int Id, int CartId, int ProductId, int Quantity, ProductDto? Product);
    public record CartDto(int Id, string UserId, List<CartItemDto> Items);

    public async Task<CartDto?> GetCartAsync()
    {
        await AttachTokenAsync();
        return await _http.GetFromJsonAsync<CartDto>("api/cart");
    }

    public async Task<HttpResponseMessage> RemoveFromCartAsync(int productId)
    {
        await AttachTokenAsync();
        return await _http.DeleteAsync($"api/cart/items/{productId}");
    }

    public async Task<HttpResponseMessage> UpdateCartItemAsync(int productId, int quantity)
    {
        await AttachTokenAsync();
        return await _http.PutAsJsonAsync($"api/cart/items/{productId}", quantity);
    }

    public async Task<List<OrderDto>?> GetMyOrdersAsync()
    {
        await AttachTokenAsync();
        return await _http.GetFromJsonAsync<List<OrderDto>>("api/orders");
    }

    public async Task<HttpResponseMessage> CancelOrderAsync(int id)
    {
        await AttachTokenAsync();
        return await _http.PutAsync($"api/orders/{id}/cancel", null);
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }

    // ---------- Wishlist ----------
    public record WishlistItemDto(int Id, string UserId, int ProductId, ProductDto? Product, DateTime AddedAt);

    public async Task<List<WishlistItemDto>?> GetWishlistAsync()
    {
        await AttachTokenAsync();
        return await _http.GetFromJsonAsync<List<WishlistItemDto>>("api/wishlist");
    }

    public async Task<HttpResponseMessage> AddToWishlistAsync(int productId)
    {
        await AttachTokenAsync();
        return await _http.PostAsync($"api/wishlist/{productId}", null);
    }

    public async Task<HttpResponseMessage> RemoveFromWishlistAsync(int productId)
    {
        await AttachTokenAsync();
        return await _http.DeleteAsync($"api/wishlist/{productId}");
    }

    // ---------- Reviews ----------
    public record ReviewDto(int Id, int ProductId, string UserId, string UserName, int Rating, string Comment, DateTime CreatedAt);
    public record ReviewsResponse(double AverageRating, int Count, List<ReviewDto> Reviews);
    public record AddReviewRequest(int Rating, string Comment);

    public async Task<ReviewsResponse?> GetReviewsAsync(int productId)
    {
        return await _http.GetFromJsonAsync<ReviewsResponse>($"api/reviews/product/{productId}");
    }

    public async Task<HttpResponseMessage> AddReviewAsync(int productId, int rating, string comment)
    {
        await AttachTokenAsync();
        return await _http.PostAsJsonAsync($"api/reviews/product/{productId}", new AddReviewRequest(rating, comment));
    }

    // ---------- Profile ----------
    public record ProfileDto(string FullName, string Email, string? Address, string? PhoneNumber);
    public record UpdateProfileRequest(string FullName, string Address, string PhoneNumber);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    public async Task<ProfileDto?> GetProfileAsync()
    {
        await AttachTokenAsync();
        return await _http.GetFromJsonAsync<ProfileDto>("api/profile");
    }

    public async Task<HttpResponseMessage> UpdateProfileAsync(string fullName, string address, string phone)
    {
        await AttachTokenAsync();
        return await _http.PutAsJsonAsync("api/profile", new UpdateProfileRequest(fullName, address, phone));
    }

    public async Task<HttpResponseMessage> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        await AttachTokenAsync();
        return await _http.PostAsJsonAsync("api/profile/change-password", new ChangePasswordRequest(currentPassword, newPassword));
    }

    public async Task<HttpResponseMessage> CheckoutAsync(string shippingAddress, string paymentMethod)
    {
        await AttachTokenAsync();
        return await _http.PostAsJsonAsync("api/orders/checkout", new { shippingAddress, paymentMethod });
    }

    // ---------- Admin-only product management (backend enforces [Authorize(Roles = "Admin")]) ----------

    public async Task<HttpResponseMessage> CreateProductAsync(ProductWriteRequest product)
    {
        await AttachTokenAsync();
        return await _http.PostAsJsonAsync("api/products", product);
    }

    public async Task<HttpResponseMessage> UpdateProductAsync(int id, ProductWriteRequest product)
    {
        await AttachTokenAsync();
        return await _http.PutAsJsonAsync($"api/products/{id}", product);
    }

    public async Task<HttpResponseMessage> DeleteProductAsync(int id)
    {
        await AttachTokenAsync();
        return await _http.DeleteAsync($"api/products/{id}");
    }

    public record OrderItemDto(int Id, int ProductId, string ProductName, decimal UnitPrice, int Quantity);
    public record OrderDto(int Id, string UserId, DateTime OrderDate, decimal TotalAmount, int Status, string ShippingAddress, string PaymentMethod, List<OrderItemDto> Items);

    public async Task<List<OrderDto>?> GetAllOrdersAsync()
    {
        await AttachTokenAsync();
        return await _http.GetFromJsonAsync<List<OrderDto>>("api/orders/all");
    }

    public async Task<HttpResponseMessage> UpdateOrderStatusAsync(int id, int status)
    {
        await AttachTokenAsync();
        return await _http.PutAsJsonAsync($"api/orders/{id}/status", status);
    }
}
