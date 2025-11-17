using CoffeeBlog_BackEnd.Models;
using CoffeeReviewApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}