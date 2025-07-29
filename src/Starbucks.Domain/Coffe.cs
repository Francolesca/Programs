using System;

namespace Starbucks.Domain;

public class Coffe : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? Imagen { get; set; }
    public Category? Category { get; set; }
    public ICollection<Ingredient> Ingredients { get; set; } = [];
    public ICollection<CoffeIngredient> CoffeIngredients { get; set; } = [];
}
