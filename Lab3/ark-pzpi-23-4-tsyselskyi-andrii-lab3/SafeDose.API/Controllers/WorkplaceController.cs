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
    public class WorkplaceController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public WorkplaceController(IApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Workplaces.ToListAsync();
            return Ok(list.Select(w => new
            {
                w.Id,
                w.Name,
                w.Address
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var entity = await _db.Workplaces.FindAsync(id);
            if (entity == null)
                return NotFound();

            return Ok(new
            {
                entity.Id,
                entity.Name,
                entity.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkplaceRequest dto)
        {
            try
            {
                var entity = new Workplace
                {
                    Name = dto.Name,
                    Address = dto.Address
                };

                _db.Workplaces.Add(entity);
                await _db.SaveChangesAsync();
                return Ok(entity.Id);
            }
            catch
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CreateWorkplaceRequest dto)
        {
            var entity = await _db.Workplaces.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                entity.Name = dto.Name;
                entity.Address = dto.Address;

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
            var entity = await _db.Workplaces.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                _db.Workplaces.Remove(entity);
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
