using System.ComponentModel.DataAnnotations;
using System.Data;
using gigu_back_end.Briefcases.Domain.Models.Commands;
using gigu_back_end.Briefcases.Domain.Models.Exceptions;
using gigu_back_end.Briefcases.Domain.Models.Queries;
using gigu_back_end.Briefcases.Domain.Services;
using gigu_back_end.Briefcases.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace gigu_back_end.Briefcases.Interfaces.REST;

[Route("api/v1/[controller]")]
[ApiController]
public class BriefcaseController(IBriefcaseQueryService briefcaseQueryService, IBriefcaseCommandService briefcaseCommandService) : ControllerBase
{
    private readonly IBriefcaseQueryService _briefcaseQueryService = briefcaseQueryService;
    private readonly IBriefcaseCommandService _briefcaseCommandService = briefcaseCommandService;

    /// <summary>
    /// Obtiene todos los portafolios.
    /// </summary>
    /// <returns>Lista de portafolios.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync()
    {
        var result = await _briefcaseQueryService.Handle(new GetAllBriefcasesQuery());
        return result.Any()
            ? Ok(result.Select(BriefcaseResourceFromEntityAssembler.ToResourceFromEntity))
            : NotFound("No briefcases found.");
    }

    /// <summary>
    /// Obtiene un portafolio por su ID.
    /// </summary>
    /// <param name="id">ID del portafolio.</param>
    /// <returns>Portafolio encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        if (id <= 0) return BadRequest("Invalid briefcase ID.");

        var result = await _briefcaseQueryService.Handle(new GetBriefcaseByIdQuery(id));
        return result != null
            ? Ok(BriefcaseResourceFromEntityAssembler.ToResourceFromEntity(result))
            : NotFound($"Briefcase with ID {id} not found.");
    }

    /// <summary>
    /// Crea un nuevo portafolio.
    /// </summary>
    /// <param name="command">Comando para crear portafolio.</param>
    /// <returns>Estado de creación.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateBriefcaseCommand command)
    {
        if (command == null) return BadRequest("Briefcase name cannot be empty.");

        try
        {
            await _briefcaseCommandService.Handle(command);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ValidationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (NotProjectFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DuplicateNameException)
        {
            return Conflict("A briefcase with the same name already exists.");
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

   
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateBriefcaseCommand command)
    {
        if (id <= 0) return BadRequest("Invalid briefcase ID.");

        try
        {
            await _briefcaseCommandService.Handle(command, id);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Elimina un portafolio por su ID.
    /// </summary>
    /// <param name="id">ID del portafolio.</param>
    /// <returns>Sin contenido si se elimina correctamente.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest("Invalid briefcase ID.");

        try
        {
            await _briefcaseCommandService.Handle(new DeleteBriefcaseCommand(id));
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
