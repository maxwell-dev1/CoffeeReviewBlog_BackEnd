using CoffeeBlog_BackEnd.Models;
using CoffeeReviewApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CoffeeBlog_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context; 

        public UsersController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


        // Optional: GET single user (for testing)
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return new User { Id = user.Id, Username = user.Username, Email = user.Email };
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(RegisterUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                Console.WriteLine("Username already exists, user not registered.");
                return BadRequest("Username already exists.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                Console.WriteLine("Email already exists, user not registered.");
                return BadRequest("Email already exists.");

            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                user.IsActive
            });
        }

        // Helper: Hash password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

    }
}
