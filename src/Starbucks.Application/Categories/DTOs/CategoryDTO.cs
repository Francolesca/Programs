using System;

namespace Starbucks.Application.Categories.DTOs;

public class CategoryDTO
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
