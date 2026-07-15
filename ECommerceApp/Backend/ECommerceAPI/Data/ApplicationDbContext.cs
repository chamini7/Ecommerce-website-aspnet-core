using ECommerceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data;

// IdentityDbContext gives us Users, Roles, Claims tables automatically.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed some sample products so the app has data immediately
        builder.Entity<Product>().HasData(
            // Mouses (2)
            new Product { Id = 1, Name = "Wireless Mouse", Description = "Ergonomic 2.4GHz wireless mouse with silent clicks", Price = 1500m, StockQuantity = 50, Category = "Mouses", ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400" },
            new Product { Id = 2, Name = "RGB Gaming Mouse", Description = "High-precision optical sensor with customizable RGB lighting", Price = 3200m, StockQuantity = 35, Category = "Mouses", ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=400" },
            // Keyboards (2)
            new Product { Id = 3, Name = "Mechanical Keyboard", Description = "RGB backlit mechanical keyboard with blue switches", Price = 8500m, StockQuantity = 30, Category = "Keyboards", ImageUrl = "https://images.unsplash.com/photo-1541140532154-b024d705b90a?w=400" },
            new Product { Id = 4, Name = "Compact Wireless Keyboard", Description = "Slim, silent wireless keyboard for office and home use", Price = 4200m, StockQuantity = 45, Category = "Keyboards", ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400" },
            // Laptops (4)
            new Product { Id = 5, Name = "Nexora Book Pro 15", Description = "Intel Core i5, 16GB RAM, 512GB SSD - built for work and study", Price = 245000m, StockQuantity = 12, Category = "Laptops", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400" },
            new Product { Id = 6, Name = "Nexora Air 14", Description = "Ultra-light laptop, AMD Ryzen 5, 8GB RAM, 256GB SSD", Price = 189000m, StockQuantity = 18, Category = "Laptops", ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400" },
            new Product { Id = 7, Name = "Nexora Gaming X15", Description = "RTX graphics, 16GB RAM, 1TB SSD - built for gaming and design", Price = 385000m, StockQuantity = 8, Category = "Laptops", ImageUrl = "https://images.unsplash.com/photo-1602080858428-57174f9431cf?w=400" },
            new Product { Id = 8, Name = "Nexora Business E5", Description = "Core i7, 16GB RAM, 1TB SSD - reliable all-day battery for professionals", Price = 298000m, StockQuantity = 10, Category = "Laptops", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400" },
            // Laptop Chargers (2)
            new Product { Id = 11, Name = "65W Laptop Charger", Description = "Universal USB-C fast charger compatible with most laptop brands", Price = 4500m, StockQuantity = 60, Category = "Laptop Chargers", ImageUrl = "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?w=400" },
            new Product { Id = 12, Name = "90W Original Charger", Description = "High-power replacement charger with 2m braided cable", Price = 6200m, StockQuantity = 40, Category = "Laptop Chargers", ImageUrl = "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?w=400" },
            // Headphones / Headsets (5)
            new Product { Id = 13, Name = "Bluetooth Headphones", Description = "Over-ear noise cancelling headphones, 30-hour battery life", Price = 12500m, StockQuantity = 25, Category = "Headphones", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400" },
            new Product { Id = 14, Name = "Gaming Headset", Description = "Surround sound headset with noise-cancelling boom microphone", Price = 7800m, StockQuantity = 22, Category = "Headphones", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400" },
            new Product { Id = 15, Name = "Wired Office Headset", Description = "Lightweight USB headset, ideal for calls and online meetings", Price = 3500m, StockQuantity = 32, Category = "Headphones", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400" },
            new Product { Id = 16, Name = "Studio Monitor Headphones", Description = "Flat-response headphones for music production and mixing", Price = 15800m, StockQuantity = 10, Category = "Headphones", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400" },
            new Product { Id = 17, Name = "Foldable Travel Headset", Description = "Compact foldable design with carrying pouch, 20-hour battery", Price = 5200m, StockQuantity = 28, Category = "Headphones", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400" },
            // Earphones (5)
            new Product { Id = 18, Name = "Wireless Earbuds Pro", Description = "True wireless earbuds with charging case, 24-hour total playtime", Price = 8900m, StockQuantity = 40, Category = "Earphones", ImageUrl = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400" },
            new Product { Id = 19, Name = "Sports Earphones", Description = "Sweat-resistant in-ear earphones with secure ear hooks", Price = 3200m, StockQuantity = 50, Category = "Earphones", ImageUrl = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400" },
            new Product { Id = 20, Name = "Wired Earphones with Mic", Description = "3.5mm in-ear earphones with built-in microphone and remote", Price = 950m, StockQuantity = 70, Category = "Earphones", ImageUrl = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400" },
            new Product { Id = 21, Name = "Noise Isolating Earphones", Description = "Deep bass in-ear earphones with memory foam tips", Price = 2400m, StockQuantity = 45, Category = "Earphones", ImageUrl = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400" },
            new Product { Id = 22, Name = "Mini Wireless Earbuds", Description = "Compact touch-control earbuds with pocket-sized case", Price = 6500m, StockQuantity = 33, Category = "Earphones", ImageUrl = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400" }
        );
    }
}
