using System;

namespace Starbucks.Domain;

public class Ingredient : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Coffe> Coffes { get; set; } = [];
    public ICollection<CoffeIngredient> CoffeIngredients { get; set; } = [];
}
