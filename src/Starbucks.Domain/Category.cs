using System;

namespace Starbucks.Domain;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Coffe> Coffes { get; set; } = [];
}
