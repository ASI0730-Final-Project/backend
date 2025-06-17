using Gigs.Domain.Models.Commands;
using Gigs.Interfaces.REST.Resources;

namespace Gigs.Interfaces.REST.Transform
{
    public static class CreateGigCommandFromResourceAssembler
    {
        public static CreateGigCommand ToCommandFromResource(CreateGigResource resource)
        {
            return new CreateGigCommand
            {
                Title = resource.Title,
                Description = resource.Description,
                Price = resource.Price,
                UserId = resource.UserId,
                Category = resource.Category,
                DeliveryDays = resource.DeliveryDays
            };
        }
    }
}