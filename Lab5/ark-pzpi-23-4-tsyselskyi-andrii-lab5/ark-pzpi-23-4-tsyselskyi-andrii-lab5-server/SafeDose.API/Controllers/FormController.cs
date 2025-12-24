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
    public class FormController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public FormController(IApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var forms = await _db.Forms.ToListAsync();
            return Ok(forms.Select(f => new { f.Id, f.Name }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var form = await _db.Forms.FindAsync(id);
            if (form == null)
                return NotFound();

            return Ok(new { form.Id, form.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNameRequest dto)
        {
            try
            {
                var entity = new Form { Name = dto.Name };
                _db.Forms.Add(entity);
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
            var entity = await _db.Forms.FindAsync(id);
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
            var entity = await _db.Forms.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                _db.Forms.Remove(entity);
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
