using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SafeDose.API.Requests;
using SafeDose.Application.Contracts.DbContext;
using SafeDose.Application.Contracts.Storage;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Models.Storage;
using SafeDose.Domain.Entities;

namespace SafeDose.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MedicationController : ControllerBase
    {
        private readonly IApplicationDbContext _db;
        private readonly IMedicineImageStorageService _medicineImageStorageService;
        private readonly DefaultFiles _defaultFiles;

        public MedicationController(IApplicationDbContext db, IMedicineImageStorageService medicineImageStorageService, IOptions<DefaultFiles> defaultFiles)
        {
            _db = db;
            _medicineImageStorageService = medicineImageStorageService;
            _defaultFiles = defaultFiles.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var meds = await _db.Medications
                .Include(m => m.Form)
                .Include(m => m.Manufacturer)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Barcode,
                    m.Description,
                    m.StorageConditions,
                    m.Contraindications,
                    m.SideEffects,
                    m.StrengthAmount,
                    m.StrengthUnit,
                    m.StrengthBase,
                    m.VolumePerBlister,
                    m.VolumePerPackage,
                    m.VolumeUnit,
                    m.PricePerBlister,
                    ImageUrl = _medicineImageStorageService.GetMedicineImageUrl(m.ImageUrl ?? _defaultFiles.DefaultMedicineImage),
                    Form = new { m.Form.Id, m.Form.Name },
                    Manufacturer = new { m.Manufacturer.Id, m.Manufacturer.Name }
                })
                .ToListAsync();

            return Ok(meds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var m = await _db.Medications
                .Include(x => x.Form)
                .Include(x => x.Manufacturer)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (m == null)
                return NotFound();

            return Ok(new
            {
                m.Id,
                m.Name,
                m.Barcode,
                m.Description,
                m.StorageConditions,
                m.Contraindications,
                m.SideEffects,
                m.StrengthAmount,
                m.StrengthUnit,
                m.StrengthBase,
                m.VolumePerBlister,
                m.VolumePerPackage,
                m.VolumeUnit,
                m.PricePerBlister,
                ImageUrl = _medicineImageStorageService.GetMedicineImageUrl(m.ImageUrl ?? _defaultFiles.DefaultMedicineImage),
                Form = new { m.Form.Id, m.Form.Name },
                Manufacturer = new { m.Manufacturer.Id, m.Manufacturer.Name }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicationRequest dto)
        {
            var form = await _db.Forms.FindAsync(dto.FormId);
            if (form == null)
                return NotFound("Form not found");

            var manufacturer = await _db.Manufacturers.FindAsync(dto.ManufacturerId);
            if (manufacturer == null)
                return NotFound("Manufacturer not found");

            try
            {
                var med = new Medication
                {
                    Name = dto.Name,
                    Barcode = dto.Barcode,
                    Description = dto.Description,
                    StorageConditions = dto.StorageConditions,
                    Contraindications = dto.Contraindications,
                    SideEffects = dto.SideEffects,
                    StrengthAmount = dto.StrengthAmount,
                    StrengthUnit = dto.StrengthUnit,
                    StrengthBase = dto.StrengthBase,
                    VolumePerBlister = dto.VolumePerBlister,
                    VolumePerPackage = dto.VolumePerPackage,
                    VolumeUnit = dto.VolumeUnit,
                    PricePerBlister = dto.PricePerBlister,
                    ManufacturerId = dto.ManufacturerId,
                    FormId = dto.FormId
                };

                _db.Medications.Add(med);
                await _db.SaveChangesAsync();

                return Ok(med);
            }
            catch
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateMedicationRequest dto)
        {
            var med = await _db.Medications.FindAsync(id);
            if (med == null)
                return NotFound();

            var form = await _db.Forms.FindAsync(dto.FormId);
            if (form == null)
                return NotFound("Form not found");

            var manufacturer = await _db.Manufacturers.FindAsync(dto.ManufacturerId);
            if (manufacturer == null)
                return NotFound("Manufacturer not found");

            try
            {
                med.Name = dto.Name;
                med.Barcode = dto.Barcode;
                med.Description = dto.Description;
                med.StorageConditions = dto.StorageConditions;
                med.Contraindications = dto.Contraindications;
                med.SideEffects = dto.SideEffects;
                med.StrengthAmount = dto.StrengthAmount;
                med.StrengthUnit = dto.StrengthUnit;
                med.StrengthBase = dto.StrengthBase;
                med.VolumePerBlister = dto.VolumePerBlister;
                med.VolumePerPackage = dto.VolumePerPackage;
                med.VolumeUnit = dto.VolumeUnit;
                med.PricePerBlister = dto.PricePerBlister;
                med.FormId = dto.FormId;
                med.ManufacturerId = dto.ManufacturerId;

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
            var med = await _db.Medications.FindAsync(id);
            if (med == null)
                return NotFound();

            try
            {
                _db.Medications.Remove(med);
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
