using System;

namespace Starbucks.Application.Coffes.DTOs;

public class CoffeResponse
{

    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; } = default!;

}
