namespace Starbucks.Application.Flavours.DTOs
{
    public class FlavorUpdateRequest
    {
        public required string Name { get; set; }
        public required Stream Stream { get; set; }
    }
}
