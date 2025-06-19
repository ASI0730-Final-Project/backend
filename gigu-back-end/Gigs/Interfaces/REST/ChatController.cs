using System.ComponentModel.DataAnnotations;
using Gigs.Domain.Models.Commands;
using Gigs.Domain.Services;
using Gigs.Interfaces.REST.Resources;
using Gigs.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace gigu_back_end.Gigs.Interfaces.REST;
[Route("api/v1/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatDomainService _chatDomainService;
    private readonly IChatQueryService _chatQueryService;

    public ChatController(IChatDomainService chatDomainService, IChatQueryService chatQueryService)
    {
        _chatDomainService = chatDomainService;
        _chatQueryService = chatQueryService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChatResource), StatusCodes.Status201Created)] // Edit
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateChatCommand command)
    {
        try
        {
            var chat = await _chatDomainService.Handle(command);
            var resource = ChatResourceFromEntityAssembler.ToResourceFromEntity(chat);
            return StatusCode(StatusCodes.Status201Created, resource);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
    // GET: /api/v1/chat/user/{userId}
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ChatResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChatsByUserId(int userId)
    {
        var chats = await _chatQueryService.Handle(new GetChatsByUserIdQuery(userId));
        if (chats == null || !chats.Any())
            return NotFound($"No chats found for user {userId}");

        var result = chats.Select(ChatResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(result);
    }
}