using Gigs.Domain;
using Gigs.Domain.Models.Entities;
using Gigs.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class PullController : ControllerBase
    {
        private readonly IPullDomainService _pullDomain;

        public PullController(IPullDomainService pullDomain)
        {
            _pullDomain = pullDomain;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pull>>> GetAll()
        {
            var pulls = await _pullDomain.GetAllPullsAsync();
            return Ok(pulls);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pull>> GetById(int id)
        {
            var pull = await _pullDomain.GetPullByIdAsync(id);
            return pull is null ? NotFound() : Ok(pull);
        }

        [HttpPost]
        public async Task<ActionResult<Pull>> Create([FromBody] Pull pull)
        {
            await _pullDomain.OpenPullAsync(pull);
            return CreatedAtAction(nameof(GetById), new { id = pull.Id }, pull);
        }

        [HttpPut("{id}/update-price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request)
        {
            try
            {
                var updatedPull = await _pullDomain.UpdatePullPriceAsync(id, request.NewPrice);
                return Ok(updatedPull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                var closedPull = await _pullDomain.ClosePullAsync(id);
                return Ok(closedPull);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class UpdatePriceRequest
        {
            public decimal NewPrice { get; set; }
        }
    }
}
