using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBlog_BackEnd.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CoffeeReview> CoffeeReviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<ReviewImage> ReviewImages { get; set; } = null!; 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoffeeReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CoffeeRe__3214EC077DBD0489");// I used EF core scaffolding, so these were ripped straight from the DB, hence the weird titles

            entity.HasIndex(e => e.CreatedAt, "IX_CoffeeReviews_CreatedAt").IsDescending();

            entity.HasIndex(e => e.Title, "IX_CoffeeReviews_Title");

            entity.HasIndex(e => e.UserId, "IX_CoffeeReviews_UserId");

            entity.Property(e => e.BrewingMethod).HasMaxLength(100);    
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.CoffeeReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_CoffeeReviews_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC074CF5B8FD");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E42B48BF11").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534094D36DB").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.ToTable("ReviewImages"); // explicit because we created it manually

            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImagePath).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.ReviewId); // tiny performance boost

            // Enforce one-to-one (only one image per review)
            entity.HasOne(e => e.CoffeeReview)
                  .WithOne(r => r.Image)
                  .HasForeignKey<ReviewImage>(e => e.ReviewId)
                  .OnDelete(DeleteBehavior.Cascade); // optional but nice
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
