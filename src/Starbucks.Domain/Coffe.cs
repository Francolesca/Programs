using System;
using Starbucks.Domain.Abstractions;

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
public static class CoffeErrors
{
    public static Error NameDuplicate = new Error
    (
        "COFFE.NAME_DUPLICATE",
        "That Coffe already exists in the system."
    );
        public static Error CoffeNoExist = new Error
    (
        "COFFE.COFFE_NOT_EXIST",
        "That Coffe does not exist in the system."
    );
}