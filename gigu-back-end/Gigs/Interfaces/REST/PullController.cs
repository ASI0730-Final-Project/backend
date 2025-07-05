using Gigs.Domain.Models.Entities;
using Gigs.Domain.Services;
using Gigs.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private static readonly HashSet<string> AllowedStates = new()
        {
            "pending",
            "in_process",
            "payed",
            "complete"
        };

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
        public async Task<ActionResult<PullResource>> Create([FromBody] CreatePullResource resource)
        {
            var pull = new Pull
            {
                SellerId = resource.SellerId,
                GigId = resource.GigId,
                PriceInit = resource.PriceInit,
                PriceUpdate = resource.PriceUpdate ?? resource.PriceInit,
                BuyerId = resource.BuyerId,
                State = resource.State ?? "pending"
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

        // PUT para actualizar precio y/o estado
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePullRequest request)
        {
            try
            {
                if (request.NewPrice.HasValue && request.NewPrice <= 0)
                    return BadRequest(new { message = "New price must be greater than zero." });

                if (!string.IsNullOrEmpty(request.NewState) && !AllowedStates.Contains(request.NewState))
                    return BadRequest(new { message = "Invalid state." });

                var updatedPull = await _pullDomain.UpdatePullAsync(id, request.NewPrice, request.NewState);

                var result = new PullResource
                {
                    Id = updatedPull.Id,
                    SellerId = updatedPull.SellerId,
                    BuyerId = updatedPull.BuyerId,
                    GigId = updatedPull.GigId,
                    PriceInit = updatedPull.PriceInit,
                    PriceUpdate = updatedPull.PriceUpdate,
                    State = updatedPull.State
                };

                return Ok(result);
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

        public class UpdatePullRequest
        {
            public decimal? NewPrice { get; set; }
            public string? NewState { get; set; }
        }
    }
}
