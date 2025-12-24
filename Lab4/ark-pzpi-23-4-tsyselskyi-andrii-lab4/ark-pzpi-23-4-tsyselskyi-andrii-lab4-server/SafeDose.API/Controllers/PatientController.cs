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
    public class PatientController : ControllerBase
    {
        private readonly IApplicationDbContext _db;
        private readonly IProfileImageStorageService _profileImageStorageService;
        private readonly DefaultFiles _defaultFiles;

        public PatientController(IApplicationDbContext db, IProfileImageStorageService profileImageStorageService, IOptions<DefaultFiles> defaultFiles)
        {
            _db = db;
            _profileImageStorageService = profileImageStorageService;
            _defaultFiles = defaultFiles.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _db.Patients
                .Include(p => p.User)
                .Select(p => new
                {
                    p.Id,
                    p.BloodType,
                    User = new
                    {
                        p.User.Id,
                        p.User.Email,
                        p.User.FirstName,
                        p.User.LastName,
                        p.User.UserName,
                        p.User.PhoneNumber,
                        AvatarUrl = _profileImageStorageService.GetProfileImageUrl(p.User.AvatarUrl ?? _defaultFiles.DefaultProfileImage),
                        p.User.Biography,
                        p.User.BirthDate
                    }
                })
                .ToListAsync();

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var p = await _db.Patients
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            return Ok(new
            {
                p.Id,
                p.BloodType,
                User = new
                {
                    p.User.Id,
                    p.User.Email,
                    p.User.FirstName,
                    p.User.LastName,
                    p.User.UserName,
                    p.User.PhoneNumber,
                    AvatarUrl = _profileImageStorageService.GetProfileImageUrl(p.User.AvatarUrl ?? _defaultFiles.DefaultProfileImage),
                    p.User.Biography,
                    p.User.BirthDate
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatientRequest dto)
        {
            var user = await _db.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            try
            {
                var patient = new Patient
                {
                    BloodType = dto.BloodType,
                    UserId = dto.UserId
                };

                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();

                return Ok(patient);
            }
            catch
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdatePatientRequest dto)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            try
            {
                patient.BloodType = dto.BloodType;
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
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            try
            {
                _db.Patients.Remove(patient);
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
