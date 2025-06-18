using System.ComponentModel.DataAnnotations;
using Gigs.Domain.Models.Commands;
using Gigs.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace gigu_back_end.Gigs.Interfaces.REST;
[Route("api/v1/[controller]")]
[ApiController]
public class ChatController(IChatDomainService chatService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateChatCommand command)
    {
        try
        {
            var message = await chatService.Handle(command);
            return StatusCode(StatusCodes.Status201Created, message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }
}