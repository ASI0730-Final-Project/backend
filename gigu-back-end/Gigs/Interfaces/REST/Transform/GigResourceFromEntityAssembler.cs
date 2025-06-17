using Gigs.Domain.Models.Entities;
using Gigs.Interfaces.REST.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Gigs.Interfaces.REST.Transform
{
    public static class GigResourceFromEntityAssembler
    {
        public static GigResource ToResourceFromEntity(Gig entity)
        {
            return new GigResource
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Price = entity.Price,
                CreatedAt = entity.CreatedAt,
                UserId = entity.UserId,
                Category = entity.Category,
                DeliveryDays = entity.DeliveryDays
            };
        }

        public static IEnumerable<GigResource> ToResourceFromEntity(IEnumerable<Gig> entities)
        {
            return entities.Select(ToResourceFromEntity);
        }
    }
}