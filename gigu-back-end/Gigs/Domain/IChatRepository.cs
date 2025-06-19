using Gigs.Domain.Models.Entities;
using gigu_back_end.Shared.Domain;

namespace Gigs.Domain;

public interface IChatRepository : IBaseRepository<Chat>
{
    Task<IEnumerable<Chat>> GetChatsByUserIdAsync(int userId);
}