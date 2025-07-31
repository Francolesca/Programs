namespace Starbucks.Api.Resources;

public class CoffeJson
{
   public Guid CoffeId {get;set;} = Guid.NewGuid();
   public string? Title { get; set; }
   public string? Description { get; set; }
   public string? Image { get; set; }
   public int Category { get; set; }
   public string[] Ingredients {get;set;} = [];

}
