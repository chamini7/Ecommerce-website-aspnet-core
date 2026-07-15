using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Wishlist is personal, so every action requires login
public class WishlistController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WishlistController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    [HttpGet]
    public async Task<ActionResult> GetWishlist()
    {
        var items = await _context.WishlistItems
            .Include(w => w.Product)
            .Where(w => w.UserId == CurrentUserId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var exists = await _context.WishlistItems
            .AnyAsync(w => w.UserId == CurrentUserId && w.ProductId == productId);

        if (!exists)
        {
            _context.WishlistItems.Add(new WishlistItem { UserId = CurrentUserId, ProductId = productId });
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == CurrentUserId && w.ProductId == productId);

        if (item == null) return NotFound();

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
