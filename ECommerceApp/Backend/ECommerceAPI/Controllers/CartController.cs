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
[Authorize] // Every action here requires a logged-in user
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    private async Task<Cart> GetOrCreateCartAsync()
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == CurrentUserId);

        if (cart == null)
        {
            cart = new Cart { UserId = CurrentUserId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    [HttpGet]
    public async Task<ActionResult<Cart>> GetCart()
    {
        var cart = await GetOrCreateCartAsync();
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddToCartDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) return NotFound("Product not found.");

        var cart = await GetOrCreateCartAsync();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        if (existingItem != null)
            existingItem.Quantity += dto.Quantity;
        else
            cart.Items.Add(new CartItem { CartId = cart.Id, ProductId = dto.ProductId, Quantity = dto.Quantity });

        await _context.SaveChangesAsync();
        // Return a simple confirmation instead of the raw Cart entity - returning the entity
        // directly risks a circular-reference crash during JSON serialization (Cart -> Items -> Product -> ...).
        return Ok(new { message = "Added to cart", productId = dto.ProductId, quantity = dto.Quantity });
    }

    [HttpPut("items/{productId}")]
    public async Task<IActionResult> UpdateItemQuantity(int productId, [FromBody] int quantity)
    {
        if (quantity < 1) return BadRequest("Quantity must be at least 1.");

        var cart = await GetOrCreateCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return NotFound();

        item.Quantity = quantity;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var cart = await GetOrCreateCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
