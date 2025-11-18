namespace CoffeeBlog_BackEnd.Models
{
    public class ReviewImage
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }                    // FK to CoffeeReview
        public string ImagePath { get; set; } = string.Empty; // e.g. "/uploads/reviews/abc123.jpg"

        // Navigation
        public CoffeeReview CoffeeReview { get; set; } = null!;
    }
}
