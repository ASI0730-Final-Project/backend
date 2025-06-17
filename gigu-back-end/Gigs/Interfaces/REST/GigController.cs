using Microsoft.AspNetCore.Mvc;
using Gigs.Application.CommandService;
using Gigs.Application.QueryService;
using Gigs.Domain.Models.Exceptions;
using Gigs.Domain.Models.Queries;
using Gigs.Interfaces.REST.Resources;
using Gigs.Interfaces.REST.Transform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GigController : ControllerBase
    {
        private readonly GigCommandService _gigCommandService;
        private readonly GigQueryService _gigQueryService;

        public GigController(GigCommandService gigCommandService, GigQueryService gigQueryService)
        {
            _gigCommandService = gigCommandService;
            _gigQueryService = gigQueryService;
        }

        [HttpPost]
        public async Task<ActionResult<GigResource>> CreateGig([FromBody] CreateGigResource resource)
        {
            try
            {
                var command = CreateGigCommandFromResourceAssembler.ToCommandFromResource(resource);
                var gig = await _gigCommandService.CreateGigAsync(command);
                var gigResource = GigResourceFromEntityAssembler.ToResourceFromEntity(gig);
                
                return CreatedAtAction(nameof(GetGigById), new { id = gig.Id }, gigResource);
            }
            catch (GigValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the gig", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GigResource>> GetGigById(int id)
        {
            try
            {
                var query = new GetGigByIdQuery(id);
                var gig = await _gigQueryService.GetGigByIdAsync(query);
                var resource = GigResourceFromEntityAssembler.ToResourceFromEntity(gig);
                
                return Ok(resource);
            }
            catch (GigNotFoundException)
            {
                return NotFound(new { message = $"Gig with id {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the gig", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GigResource>>> GetAllGigs(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string searchTerm = "")
        {
            try
            {
                var query = new GetAllGigsQuery(page, pageSize, searchTerm);
                var gigs = await _gigQueryService.GetAllGigsAsync(query);
                var resources = GigResourceFromEntityAssembler.ToResourceFromEntity(gigs);
                
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving gigs", details = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GigResource>>> GetGigsByUserId(int userId)
        {
            try
            {
                var query = new GetGigsByUserIdQuery(userId);
                var gigs = await _gigQueryService.GetGigsByUserIdAsync(query);
                var resources = GigResourceFromEntityAssembler.ToResourceFromEntity(gigs);
                
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user's gigs", details = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<GigResource>>> GetGigsByCategory(
            string category, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new GetGigsByCategoryQuery(category, page, pageSize);
                var gigs = await _gigQueryService.GetGigsByCategoryAsync(query);
                var resources = GigResourceFromEntityAssembler.ToResourceFromEntity(gigs);
                
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving gigs by category", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GigResource>> UpdateGig(int id, [FromBody] UpdateGigResource resource)
        {
            try
            {
                var command = UpdateGigCommandFromResourceAssembler.ToCommandFromResource(resource, id);
                var gig = await _gigCommandService.UpdateGigAsync(command);
                var gigResource = GigResourceFromEntityAssembler.ToResourceFromEntity(gig);
                
                return Ok(gigResource);
            }
            catch (GigNotFoundException)
            {
                return NotFound(new { message = $"Gig with id {id} not found" });
            }
            catch (GigValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the gig", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGig(int id, [FromQuery] int userId)
        {
            try
            {
                var result = await _gigCommandService.DeleteGigAsync(id, userId);
                if (result)
                {
                    return NoContent();
                }
                return NotFound(new { message = $"Gig with id {id} not found" });
            }
            catch (GigValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the gig", details = ex.Message });
            }
        }

        [HttpGet("stats/category/{category}/count")]
        public async Task<ActionResult<int>> GetGigCountByCategory(string category)
        {
            try
            {
                var count = await _gigQueryService.GetGigCountByCategoryAsync(category);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while getting gig count", details = ex.Message });
            }
        }

        [HttpGet("stats/user/{userId}/count")]
        public async Task<ActionResult<int>> GetGigCountByUserId(int userId)
        {
            try
            {
                var count = await _gigQueryService.GetGigCountByUserIdAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while getting user's gig count", details = ex.Message });
            }
        }

        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> GigExists(int id)
        {
            try
            {
                var exists = await _gigQueryService.GigExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking gig existence", details = ex.Message });
            }
        }
    }
}
             