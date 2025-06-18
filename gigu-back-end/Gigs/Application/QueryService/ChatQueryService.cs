using Gigs.Domain;
using Gigs.Domain.Models.Entities;
using Gigs.Domain.Models.Commands;
using Gigs.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Gigs.Application.QueryService;

public class ChatQueryService(IChatRepository chatRepository) : IChatQueryService
{
    private readonly IChatRepository _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));

    public async Task<IEnumerable<Chat>> Handle(GetChatsByUserIdQuery query)
    {
        var chats = await _chatRepository.GetChatsByUserIdAsync(query.UserId);
        return chats;
    }
}