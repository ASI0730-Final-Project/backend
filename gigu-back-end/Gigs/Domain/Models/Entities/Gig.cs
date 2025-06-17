using System;
using System.ComponentModel.DataAnnotations;

namespace Gigs.Domain.Models.Entities
{
    public class Gig
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        
        [Required]
        [Range(1, 365)]
        public int DeliveryDays { get; set; }

        public Gig()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public Gig(string title, string description, decimal price, int userId, string category, int deliveryDays)
        {
            Title = title;
            Description = description;
            Price = price;
            UserId = userId;
            Category = category;
            DeliveryDays = deliveryDays;
            CreatedAt = DateTime.UtcNow;
        }
    }
}