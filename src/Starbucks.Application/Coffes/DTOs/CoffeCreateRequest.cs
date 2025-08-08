using System;

namespace Starbucks.Application.Coffes.DTOs;

public class CoffeCreateRequest
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    public string Imagen { get; set; } = default!;
}
