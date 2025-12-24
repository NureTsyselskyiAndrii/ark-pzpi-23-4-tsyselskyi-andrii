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
    [Authorize(Roles = "Admin, Doctor")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public MedicalRecordController(IApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.MedicalRecords
                .Include(r => r.Patient)
                .Select(r => new
                {
                    r.Id,
                    r.Description,
                    Patient = new
                    {
                        r.Patient.Id,
                        r.Patient.UserId
                    }
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var r = await _db.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (r == null)
                return NotFound();

            return Ok(new
            {
                r.Id,
                r.Description,
                Patient = new
                {
                    r.Patient.Id,
                    r.Patient.UserId
                }
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordRequest dto)
        {
            var patient = await _db.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            try
            {
                var record = new MedicalRecord
                {
                    Description = dto.Description,
                    PatientId = dto.PatientId
                };

                _db.MedicalRecords.Add(record);
                await _db.SaveChangesAsync();

                return Ok(record);
            }
            catch
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateMedicalRecordRequest dto)
        {
            var record = await _db.MedicalRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            try
            {
                record.Description = dto.Description;
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                throw new InternalServerException();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var record = await _db.MedicalRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            try
            {
                _db.MedicalRecords.Remove(record);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                throw new InternalServerException();
            }
        }
    }
}
