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
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    // Public - anyone can read reviews for a product
    [HttpGet("product/{productId}")]
    public async Task<ActionResult> GetReviewsForProduct(int productId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var average = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        return Ok(new { averageRating = Math.Round(average, 1), count = reviews.Count, reviews });
    }

    // Only logged-in users can write a review
    [HttpPost("product/{productId}")]
    [Authorize]
    public async Task<ActionResult<Review>> AddReview(int productId, AddReviewDto dto)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound("Product not found.");

        var fullName = User.FindFirstValue("fullName") ?? "Anonymous";

        var review = new Review
        {
            ProductId = productId,
            UserId = CurrentUserId,
            UserName = fullName,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return Ok(review);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return NotFound();
        if (review.UserId != CurrentUserId) return Forbid();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
