using System;
using Microsoft.EntityFrameworkCore;
using Starbucks.Domain;

namespace Starbucks.Persistence;

public class StarbucksDbContext : DbContext
{
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<Coffe> Coffes { get; set; }
    public required DbSet<Ingredient> Ingredients { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Coffes)
            .WithOne(co => co.Category)
            .HasForeignKey(co => co.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Coffe>()
            .Property(co => co.Price)
            .HasPrecision(10, 2);
        modelBuilder.Entity<Coffe>()
            .HasMany(co => co.Ingredients)
            .WithMany(co => co.Coffes)
            .UsingEntity<CoffeIngredient>(
                j => j
                        .HasOne(p => p.Ingredient)
                        .WithMany(p => p.CoffeIngredients)
                        .HasForeignKey(p => p.IngredientId),
                j => j
                        .HasOne(p => p.Coffe)
                        .WithMany(p => p.CoffeIngredients)
                        .HasForeignKey(p => p.CoffeId),
                j =>
                {
                    j.HasKey(t => new { t.CoffeId, t.IngredientId });
                }
            );
    }
}
