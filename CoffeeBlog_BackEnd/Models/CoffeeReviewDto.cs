namespace CoffeeReviewApi.Models;

public class CoffeeReviewDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Rating { get; set; }
    public string Body { get; set; } = null!;
    public string? BrewingMethod { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; } = null!;
}