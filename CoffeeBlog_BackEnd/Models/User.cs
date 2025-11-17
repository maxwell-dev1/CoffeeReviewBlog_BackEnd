using System;
using System.Collections.Generic;

namespace CoffeeBlog_BackEnd.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CoffeeReview> CoffeeReviews { get; set; } = new List<CoffeeReview>();
}
