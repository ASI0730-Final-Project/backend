using Gigs.Domain.Models.Commands;
using Gigs.Domain.Models.Entities;

namespace Gigs.Domain.Services;

public interface IChatDomainService
{
    Task<Chat> Handle(CreateChatCommand command);
}