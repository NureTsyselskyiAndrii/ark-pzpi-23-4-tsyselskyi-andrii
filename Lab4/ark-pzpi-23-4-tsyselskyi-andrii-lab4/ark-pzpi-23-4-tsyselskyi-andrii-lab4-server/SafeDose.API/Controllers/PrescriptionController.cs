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
    public class PrescriptionController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public PrescriptionController(IApplicationDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var prescriptions = await _db.Prescriptions
                .Include(p => p.Patient).ThenInclude(u => u.User)
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.PrescriptionMedications).ThenInclude(pm => pm.Medication)
                .ToListAsync();

            return Ok(prescriptions.Select(p => new
            {
                p.Id,
                p.StartDate,
                p.EndDate,

                Patient = new
                {
                    p.Patient.Id,
                    p.Patient.User.FirstName,
                    p.Patient.User.LastName
                },

                Doctor = new
                {
                    p.Doctor.Id,
                    p.Doctor.User.FirstName,
                    p.Doctor.User.LastName
                },

                Medications = p.PrescriptionMedications.Select(pm => new
                {
                    pm.Id,
                    pm.MedicationId,
                    MedicationName = pm.Medication.Name,
                    pm.Dosage,
                    pm.QuantityOfDosagesOverall,
                    pm.PeriodInDays,
                    pm.Description,
                    pm.Discount
                })
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var p = await _db.Prescriptions
                .Include(o => o.Patient).ThenInclude(u => u.User)
                .Include(o => o.Doctor).ThenInclude(d => d.User)
                .Include(o => o.PrescriptionMedications).ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (p == null)
                return NotFound();

            return Ok(new
            {
                p.Id,
                p.StartDate,
                p.EndDate,

                Patient = new
                {
                    p.Patient.Id,
                    p.Patient.User.FirstName,
                    p.Patient.User.LastName
                },

                Doctor = new
                {
                    p.Doctor.Id,
                    p.Doctor.User.FirstName,
                    p.Doctor.User.LastName
                },

                Medications = p.PrescriptionMedications.Select(pm => new
                {
                    pm.Id,
                    pm.MedicationId,
                    MedicationName = pm.Medication.Name,
                    pm.Dosage,
                    pm.QuantityOfDosagesOverall,
                    pm.PeriodInDays,
                    pm.Description,
                    pm.Discount
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionRequest dto)
        {
            try
            {
                var p = new Prescription
                {
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId
                };

                _db.Prescriptions.Add(p);
                await _db.SaveChangesAsync();

                return Ok(p.Id);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdatePrescriptionRequest dto)
        {
            var p = await _db.Prescriptions.FindAsync(id);
            if (p == null)
                return NotFound();

            try
            {
                p.StartDate = dto.StartDate;
                p.EndDate = dto.EndDate;
                p.PatientId = dto.PatientId;
                p.DoctorId = dto.DoctorId;

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
            var p = await _db.Prescriptions.FindAsync(id);
            if (p == null)
                return NotFound();

            try
            {
                _db.Prescriptions.Remove(p);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            return NoContent();
        }

        [HttpPost("{id}/medications")]
        public async Task<IActionResult> AddMedication(long id, [FromBody] AddMedicationToPrescriptionRequest dto)
        {
            var prescription = await _db.Prescriptions
                .Include(p => p.PrescriptionMedications)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            try
            {
                var pm = new PrescriptionMedication
                {
                    PrescriptionId = id,
                    MedicationId = dto.MedicationId,
                    Dosage = dto.Dosage,
                    QuantityOfDosagesOverall = dto.QuantityOfDosagesOverall,
                    PeriodInDays = dto.PeriodInDays,
                    Description = dto.Description,
                    Discount = dto.Discount
                };

                prescription.PrescriptionMedications.Add(pm);
                await _db.SaveChangesAsync();

                return Ok(pm.Id);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{prescriptionId}/medications/{pmId}")]
        public async Task<IActionResult> UpdateMedication(long prescriptionId, long pmId, [FromBody] UpdatePrescriptionMedicationRequest dto)
        {
            var pm = await _db.PrescriptionMedications.FirstOrDefaultAsync(pm => pm.Id == pmId && pm.PrescriptionId == prescriptionId);

            if (pm == null)
                return NotFound();

            try
            {
                pm.Dosage = dto.Dosage;
                pm.QuantityOfDosagesOverall = dto.QuantityOfDosagesOverall;
                pm.PeriodInDays = dto.PeriodInDays;
                pm.Description = dto.Description;
                pm.Discount = dto.Discount;

                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            return NoContent();
        }


        [HttpDelete("{prescriptionId}/medications/{pmId}")]
        public async Task<IActionResult> RemoveMedication(long prescriptionId, long pmId)
        {
            var pm = await _db.PrescriptionMedications.FirstOrDefaultAsync(pm => pm.Id == pmId && pm.PrescriptionId == prescriptionId);

            if (pm == null)
                return NotFound();

            try
            {
                _db.PrescriptionMedications.Remove(pm);
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
