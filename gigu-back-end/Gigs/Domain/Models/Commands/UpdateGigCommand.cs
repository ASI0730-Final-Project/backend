namespace Gigs.Domain.Models.Commands
{
    public class UpdateGigCommand
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DeliveryDays { get; set; }
    }
}