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

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:CoffeeBlogDb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoffeeReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CoffeeRe__3214EC077DBD0489");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
