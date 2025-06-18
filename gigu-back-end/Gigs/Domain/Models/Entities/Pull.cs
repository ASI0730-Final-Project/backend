namespace Gigs.Domain.Models.Entities
{
    public class Pull
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public int? BuyerId { get; set; }
        public int GigId { get; set; }

        public decimal PriceInit { get; set; }
        public decimal PriceUpdate { get; set; }
        public string State { get; set; }  // "abierta", "cerrada", etc.
    }
}
