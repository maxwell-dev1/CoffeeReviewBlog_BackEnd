using System;
using System.Collections.Generic;

namespace CoffeeBlog_BackEnd.Models;

public partial class CoffeeReview
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public decimal Rating { get; set; }

    public string Body { get; set; } = null!;

    public string? BrewingMethod { get; set; }

    public string? ImageUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
