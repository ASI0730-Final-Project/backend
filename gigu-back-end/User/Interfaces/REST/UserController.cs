using System.ComponentModel.DataAnnotations;
using FluentValidation;
using gigu_back_end.Shared.Domain.Models.Commands;
using gigu_back_end.Shared.Infraestructure.Attribute;
using gigu_back_end.User.Domain.Models.Commands;
using gigu_back_end.User.Domain.Models.Exceptions;
using gigu_back_end.User.Domain.Services;
using gigu_back_end.User.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gigu_back_end.User.Interfaces.REST;

[Route("api/v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController(IUserQueryService userQueryService, IUserCommandService userCommandService) : ControllerBase
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IUserCommandService _userCommandService = userCommandService;
    
    [HttpGet]
    [CustomAuthorize("buyer")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userQueryService.Handle(new GetAllUsersQuery());
        return Ok(users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }
    
    
    [HttpGet("{id:int}")]
    [CustomAuthorize("seller")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userQueryService.Handle(new GetUserByIdQuery(id));
        if (user is null) return NotFound();
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        try
        {
            await _userCommandService.Handle(command);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (NotRequiredFieldsCompletedException e)
        {
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
    {
        try
        {
            await _userCommandService.Handle(command, id);
            return Ok();
        }
        catch (Exception e)
        {
            return Problem(detail: e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userCommandService.Handle(new DeleteUserCommand(id));
        return NoContent();
    }
    
    
    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        try
        {
            var user = await _userCommandService.Handle(command);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (EmailAlreadyTakenException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred", detail = ex.Message });
        }
    }
    
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var jwToken = await _userCommandService.Handle(command);
            return Ok(jwToken);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred", detail = ex.Message });
        }
    }
}
