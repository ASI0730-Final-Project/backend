namespace Gigs.Domain.Models.Commands
{
    public class CreateGigCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; }
        public int DeliveryDays { get; set; }
    }
}