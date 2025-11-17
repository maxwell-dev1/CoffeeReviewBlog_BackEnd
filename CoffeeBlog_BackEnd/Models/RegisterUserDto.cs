// Models/RegisterUserDto.cs
namespace CoffeeReviewApi.Models;

public class RegisterUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}