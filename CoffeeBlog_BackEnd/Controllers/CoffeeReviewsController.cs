using CoffeeBlog_BackEnd.Models;
using CoffeeReviewApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoffeeReviewApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoffeeReviewsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CoffeeReviewsController(AppDbContext db) => _db = db;

    // GET: api/coffeereviews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoffeeReviewDto>>> GetAll()
    {
        return await _db.CoffeeReviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CoffeeReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Rating = r.Rating,
                Body = r.Body,
                BrewingMethod = r.BrewingMethod,
                ImageUrl = r.ImageUrl,
                CreatedAt = r.CreatedAt,
                Username = r.User.Username
            })
            .ToListAsync();
    }

    // GET: api/coffeereviews/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CoffeeReviewDto>> Get(int id)
    {
        var dto = await _db.CoffeeReviews
            .Where(r => r.Id == id)
            .Select(r => new CoffeeReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Rating = r.Rating,
                Body = r.Body,
                BrewingMethod = r.BrewingMethod,
                ImageUrl = r.ImageUrl,
                CreatedAt = r.CreatedAt,
                Username = r.User.Username
            })
            .FirstOrDefaultAsync();

        if (dto == null) return NotFound();

        return dto;
    }








    // ---------------------------------------------------------------------------Image upload method ----------------------------------
    [HttpPost("{id}/image")]
    [Authorize]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // 1. Find the review + eager load current image (if any)
        var review = await _db.CoffeeReviews
            .Include(r => r.Image)
            .Include(r => r.User)           // Needed to check ownership
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
            return NotFound("Review not found.");

        // 2. Security: Only the owner can upload an image
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (review.UserId != currentUserId)
            return Forbid("You can only upload images to your own reviews.");

        // 3. Enforce one image per review
        if (review.Image != null)
            return BadRequest("This review already has an image.");

        // 4. File validation
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only images are allowed.");

        if (file.Length > 10 * 1024 * 1024) // 10 MB max
            return BadRequest("File too large. Maximum size is 10 MB.");

        // 5. Save file with unique name
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews");
        Directory.CreateDirectory(uploadsFolder); // ensure folder exists

        var filePath = Path.Combine(uploadsFolder, fileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 6. Save path to ReviewImages table
        var imagePath = $"/uploads/reviews/{fileName}";
        var reviewImage = new ReviewImage
        {
            ReviewId = review.Id,
            ImagePath = imagePath
        };

        _db.ReviewImages.Add(reviewImage);
        await _db.SaveChangesAsync();

        // 7. Return full accessible URL
        var imageUrl = $"{Request.Scheme}://{Request.Host}{imagePath}";

        return Ok(new
        {
            message = "Image uploaded successfully!",
            imageUrl
        });
    }
}