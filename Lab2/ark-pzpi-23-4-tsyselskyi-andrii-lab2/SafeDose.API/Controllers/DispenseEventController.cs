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
    public class DispenseEventController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public DispenseEventController(IApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.DispenseEvents
                .Include(e => e.Prescription)
                .Include(e => e.Device)
                .Include(e => e.Patient)
                .ThenInclude(p => p.User)
                .Include(e => e.Doctor)
                .ThenInclude(d => d.User)
                .Include(e => e.Medication)
                .ToListAsync();

            return Ok(events.Select(e => new
            {
                e.Id,
                e.QuantityDispensed,
                e.DispensedAt,
                e.Price,

                Prescription = new { e.Prescription.Id },
                Device = new { e.Device.Id, e.Device.Name },
                Patient = new { e.Patient.Id, e.Patient.User.FirstName, e.Patient.User.LastName },
                Doctor = new { e.Doctor.Id, e.Doctor.LicenseNumber, e.Doctor.User.FirstName, e.Doctor.User.LastName },
                Medication = new { e.Medication.Id, e.Medication.Name }
            }));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var e = await _db.DispenseEvents
                .Include(o => o.Prescription)
                .Include(o => o.Device)
                .Include(o => o.Patient)
                .ThenInclude(p => p.User)
                .Include(o => o.Doctor)
                .ThenInclude(p => p.User)
                .Include(o => o.Medication)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (e == null)
                return NotFound();

            return Ok(new
            {
                e.Id,
                e.QuantityDispensed,
                e.DispensedAt,
                e.Price,

                Prescription = new { e.Prescription.Id },
                Device = new { e.Device.Id, e.Device.Name },
                Patient = new { e.Patient.Id, e.Patient.User.FirstName, e.Patient.User.LastName },
                Doctor = new { e.Doctor.Id, e.Doctor.LicenseNumber, e.Doctor.User.FirstName, e.Doctor.User.LastName },
                Medication = new { e.Medication.Id, e.Medication.Name }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDispenseEventRequest dto)
        {
            try
            {
                var entity = new DispenseEvent
                {
                    QuantityDispensed = dto.QuantityDispensed,
                    DispensedAt = dto.DispensedAt ?? DateTime.UtcNow,
                    Price = dto.Price,

                    PrescriptionId = dto.PrescriptionId,
                    DeviceId = dto.DeviceId,
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    MedicationId = dto.MedicationId
                };

                _db.DispenseEvents.Add(entity);
                await _db.SaveChangesAsync();

                return Ok(entity.Id);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDispenseEventRequest dto)
        {
            var entity = await _db.DispenseEvents.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                entity.QuantityDispensed = dto.QuantityDispensed;
                entity.DispensedAt = dto.DispensedAt ?? entity.DispensedAt;
                entity.Price = dto.Price;

                entity.PrescriptionId = dto.PrescriptionId;
                entity.DeviceId = dto.DeviceId;
                entity.PatientId = dto.PatientId;
                entity.DoctorId = dto.DoctorId;
                entity.MedicationId = dto.MedicationId;

                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _db.DispenseEvents.FindAsync(id);
            if (entity == null)
                return NotFound();

            try
            {
                _db.DispenseEvents.Remove(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            return NoContent();
        }
    }
}
