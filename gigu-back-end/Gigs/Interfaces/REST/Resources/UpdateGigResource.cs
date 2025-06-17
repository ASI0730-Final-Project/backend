namespace Gigs.Interfaces.REST.Resources
{
    public class UpdateGigResource
    {
        public string Title { get; set; } = string.Empty;  
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DeliveryDays { get; set; }
    }
}