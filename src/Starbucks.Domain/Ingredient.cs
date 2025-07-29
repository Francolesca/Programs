using System;

namespace Starbucks.Domain;

public class Ingredient : BaseEntity
{
    public required string Name{ get; set; }
}
