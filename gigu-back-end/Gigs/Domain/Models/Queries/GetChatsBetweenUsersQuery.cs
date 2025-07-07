namespace Gigs.Domain.Models.Commands;

public record GetChatsBetweenUsersQuery(int SenderId, int ReceiverId);