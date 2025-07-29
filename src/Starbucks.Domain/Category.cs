using System;
using System.Diagnostics.CodeAnalysis;

namespace Starbucks.Domain;

public class Category
{
    [SetsRequiredMembers]
    private Category(int id, string name) => (Id, Name) = (id, name);
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Coffe> Coffes { get; set; } = [];

    public static Category Create(int id)
    {
        var categoryName = (CategoryEnum)id;
        var categoryNameString = categoryName.ToString();
        return new Category(id, categoryNameString);
        
    }
}
