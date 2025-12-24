using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeDose.API.Requests;
using SafeDose.Application.Contracts.DbContext;
using SafeDose.Application.Exceptions;
using SafeDose.Domain.Entities;

namespace SafeDose.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PositionController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public PositionController(IApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Positions.ToListAsync();
            return Ok(list.Select(p => new { p.Id, p.Name }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var entity = await _db.Positions.FindAsync(id);
            if (entity == null)
                return NotFound();

            return Ok(new { entity.Id, entity.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNameRequest dto)
        {
            try
            {
                var entity = new Position { Name = dto.Name };
                _db.Positions.Add(entity);
                await _db.SaveChangesAsync();
                return Ok(entity.Id);
            }
            catch
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CreateNameRequest dto)
        {
            var entity = await _db.Positions.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                entity.Name = dto.Name;
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _db.Positions.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                _db.Positions.Remove(entity);
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException();
            }

            return NoContent();
        }
    }
}
