using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    // Turns the current cart into an Order - classic e-commerce checkout flow
    [HttpPost("checkout")]
    public async Task<ActionResult<Order>> Checkout(CheckoutDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

        if (cart == null || !cart.Items.Any())
            return BadRequest("Your cart is empty.");

        var order = new Order
        {
            UserId = CurrentUserId,
            ShippingAddress = dto.ShippingAddress,
            PaymentMethod = dto.PaymentMethod,
            Status = OrderStatus.Pending,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product!.Name,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        // Reduce stock for each purchased product
        foreach (var item in cart.Items)
        {
            item.Product!.StockQuantity = Math.Max(0, item.Product.StockQuantity - item.Quantity);
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.Items); // empty the cart after checkout
        await _context.SaveChangesAsync();

        return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult> GetMyOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == CurrentUserId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == CurrentUserId);

        if (order == null) return NotFound();
        return Ok(order);
    }

    // A customer can only cancel their own order, and only while it's still Pending
    // (once the admin starts processing it, cancelling would need a support request instead)
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == CurrentUserId);

        if (order == null) return NotFound();
        if (order.Status != OrderStatus.Pending)
            return BadRequest("Only pending orders can be cancelled.");

        // Restore stock since the order never shipped
        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null) product.StockQuantity += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Admin-only: view all orders, update status (Pending -> Shipped -> Delivered)
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAllOrders()
    {
        var orders = await _context.Orders.Include(o => o.Items).ToListAsync();
        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
