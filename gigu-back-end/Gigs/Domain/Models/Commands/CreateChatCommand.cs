namespace Gigs.Domain.Models.Commands;

public record CreateChatCommand(int SenderId, int ReceiverId, string Content);
