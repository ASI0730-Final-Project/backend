using System;

namespace Gigs.Interfaces.REST.Resources
{
    public class GigResource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; }
        public int DeliveryDays { get; set; }
    }
}