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

      
        // GET: api/Book
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _briefcaseQueryService.Handle(new GetAllBriefcasesQuery());
            return result.Any() ? Ok(result.Select(BriefcaseResourceFromEntityAssembler.ToResourceFromEntity)) : NotFound("No briefcases found.");
        }
        
     
       
        // GET: api/Book/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0) return BadRequest("Invalid briefcase ID.");

            var result = await _briefcaseQueryService.Handle(new GetBriefcaseByIdQuery(id));
            return result != null ? Ok(BriefcaseResourceFromEntityAssembler.ToResourceFromEntity(result)) : NotFound($"Briefcase with ID {id} not found.");
        }

        
        
       
        
        // POST: api/Book
        [HttpPost]
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

        // PUT: api/Book/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateBriefcaseCommand command)
        {
            if (id <= 0) return BadRequest("Invalid briefcase ID.");

            try
            {
                await _briefcaseCommandService.Handle(command, id);
                return Ok();
            }
            catch (Exception ex) { return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError); }
        }

        // DELETE: api/Book/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid briefcase ID.");

            try
            {
                await _briefcaseCommandService.Handle(new DeleteBriefcaseCommand(id));
                return NoContent();
            }
            catch (Exception ex) { return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError); }
        }
    }

