using Gigs.Domain.Models.Entities;
using Gigs.Domain.Services;
using Gigs.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<IEnumerable<PullResource>>> GetAll()
        {
            var pulls = await _pullDomain.GetAllPullsAsync();

            var resources = pulls.Select(p => new PullResource
            {
                Id = p.Id,
                SellerId = p.SellerId,
                BuyerId = p.BuyerId,
                GigId = p.GigId,
                PriceInit = p.PriceInit,
                PriceUpdate = p.PriceUpdate,
                State = p.State
            });

            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PullResource>> GetById(int id)
        {
            var pull = await _pullDomain.GetPullByIdAsync(id);
            if (pull == null) return NotFound();

            var resource = new PullResource
            {
                Id = pull.Id,
                SellerId = pull.SellerId,
                BuyerId = pull.BuyerId,
                GigId = pull.GigId,
                PriceInit = pull.PriceInit,
                PriceUpdate = pull.PriceUpdate,
                State = pull.State
            };

            return Ok(resource);
        }

        [HttpPost]
        public async Task<ActionResult<PullResource>> Create([FromBody] SavePullResource resource)
        {
            var pull = new Pull
            {
                SellerId = resource.SellerId,
                GigId = resource.GigId,
                PriceInit = resource.PriceInit,
                PriceUpdate = resource.PriceUpdate ?? resource.PriceInit,
                BuyerId = resource.BuyerId,
                State = resource.State ?? "abierta"
            };

            await _pullDomain.OpenPullAsync(pull);

            var result = new PullResource
            {
                Id = pull.Id,
                SellerId = pull.SellerId,
                BuyerId = pull.BuyerId,
                GigId = pull.GigId,
                PriceInit = pull.PriceInit,
                PriceUpdate = pull.PriceUpdate,
                State = pull.State
            };

            return CreatedAtAction(nameof(GetById), new { id = pull.Id }, result);
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
