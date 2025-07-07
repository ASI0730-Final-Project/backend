using Gigs.Domain.Models.Commands;
using Gigs.Domain.Models.Entities;

namespace Gigs.Domain.Services;

public interface IChatQueryService
{
    Task<IEnumerable<Chat>> Handle(GetChatsByUserIdQuery query);
    Task<IEnumerable<Chat>> Handle(GetChatsBetweenUsersQuery query);

}
