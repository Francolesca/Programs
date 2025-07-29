using System;

namespace Starbucks.Domain;

public class CoffeIngredient
{
    public Guid IngredientId { get; set; }
    public Guid CoffeId { get; set; }
}
