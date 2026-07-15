using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs;

public class ProductDto
{
    [Required] public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Range(0.01, 1000000)] public decimal Price { get; set; }
    [Range(0, int.MaxValue)] public int StockQuantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class AddToCartDto
{
    [Required] public int ProductId { get; set; }
    [Range(1, 1000)] public int Quantity { get; set; } = 1;
}

public class CheckoutDto
{
    [Required] public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "Cash on Delivery";
}

public class AddReviewDto
{
    [Range(1, 5)] public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    [Required] public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    [Required] public string CurrentPassword { get; set; } = string.Empty;
    [Required, MinLength(6)] public string NewPassword { get; set; } = string.Empty;
}
