using Gigs.Domain;
using Gigs.Domain.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Gigs.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class PullController : ControllerBase
    {
        private readonly IPullRepository _pullRepo;

        public PullController(IPullRepository pullRepo)
        {
            _pullRepo = pullRepo;
        }

        // GET: api/pull
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pull>>> GetAll()
        {
            var pulls = await _pullRepo.GetAllAsync();
            return Ok(pulls);
        }

        // POST: api/pull
        [HttpPost]
        public async Task<ActionResult<Pull>> Create([FromBody] Pull pull)
        {
            pull.State = "abierta";
            await _pullRepo.CreateAsync(pull);
            await _pullRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pull.Id }, pull);
        }

        // GET: api/pull/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Pull>> GetById(int id)
        {
            var pull = await _pullRepo.GetByIdAsync(id);
            if (pull == null) return NotFound();

            return Ok(pull);
        }

        // PUT: api/pull/{id}/update-price
        [HttpPut("{id}/update-price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request)
        {
            var pull = await _pullRepo.GetByIdAsync(id);
            if (pull == null) return NotFound();

            if (pull.State != "abierta")
                return BadRequest("La subasta ya está cerrada");

            if (request.NewPrice <= pull.PriceUpdate)
                return BadRequest("El nuevo precio debe ser mayor al actual");

            pull.PriceUpdate = request.NewPrice;
            await _pullRepo.UpdateAsync(pull);
            await _pullRepo.SaveChangesAsync();
            return Ok(pull);
        }

        // PUT: api/pull/{id}/close
        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var pull = await _pullRepo.GetByIdAsync(id);
            if (pull == null) return NotFound();

            if (pull.State == "cerrada")
                return BadRequest("La subasta ya está cerrada");

            pull.State = "cerrada";
            await _pullRepo.UpdateAsync(pull);
            await _pullRepo.SaveChangesAsync();
            return Ok(pull);
        }
    }

    // DTO para actualizar el precio
    public class UpdatePriceRequest
    {
        public decimal NewPrice { get; set; }
    }
}
