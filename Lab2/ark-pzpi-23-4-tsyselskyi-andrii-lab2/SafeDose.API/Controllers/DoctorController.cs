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
    public class DoctorController : ControllerBase
    {
        private readonly IApplicationDbContext _db;
        private readonly IProfileImageStorageService _profileImageStorageService;
        private readonly DefaultFiles _defaultFiles;

        public DoctorController(IApplicationDbContext db, IProfileImageStorageService profileImageStorageService, IOptions<DefaultFiles> defaultFiles)
        {
            _db = db;
            _profileImageStorageService = profileImageStorageService;
            _defaultFiles = defaultFiles.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _db.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Workplace)
                .Include(d => d.Position)
                .Include(d => d.User)
                .ToListAsync();

            return Ok(doctors.Select(d => new
            {
                d.Id,
                d.LicenseNumber,

                Specialization = new { d.Specialization.Id, d.Specialization.Name },
                Workplace = new { d.Workplace.Id, d.Workplace.Name },
                Position = new { d.Position.Id, d.Position.Name },

                User = new
                {
                    d.User.Id,
                    d.User.FirstName,
                    d.User.LastName,
                    d.User.Email,
                    d.User.PhoneNumber,
                    AvatarUrl = _profileImageStorageService.GetProfileImageUrl(d.User.AvatarUrl ?? _defaultFiles.DefaultProfileImage),
                    d.User.UserName,
                    d.User.Biography,
                    d.User.BirthDate
                }
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var d = await _db.Doctors
                .Include(o => o.Specialization)
                .Include(o => o.Workplace)
                .Include(o => o.Position)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (d == null)
                return NotFound();

            return Ok(new
            {
                d.Id,
                d.LicenseNumber,

                Specialization = new { d.Specialization.Id, d.Specialization.Name },
                Workplace = new { d.Workplace.Id, d.Workplace.Name },
                Position = new { d.Position.Id, d.Position.Name },

                User = new
                {
                    d.User.Id,
                    d.User.FirstName,
                    d.User.LastName,
                    d.User.Email,
                    d.User.PhoneNumber,
                    AvatarUrl = _profileImageStorageService.GetProfileImageUrl(d.User.AvatarUrl ?? _defaultFiles.DefaultProfileImage),
                    d.User.UserName,
                    d.User.Biography,
                    d.User.BirthDate
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorRequest dto)
        {
            try
            {
                var doctor = new Doctor
                {
                    LicenseNumber = dto.LicenseNumber,
                    SpecializationId = dto.SpecializationId,
                    WorkplaceId = dto.WorkplaceId,
                    PositionId = dto.PositionId,
                    UserId = dto.UserId
                };

                _db.Doctors.Add(doctor);
                await _db.SaveChangesAsync();

                return Ok(doctor.Id);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDoctorRequest dto)
        {
            var doctor = await _db.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            try
            {
                doctor.LicenseNumber = dto.LicenseNumber;
                doctor.SpecializationId = dto.SpecializationId;
                doctor.WorkplaceId = dto.WorkplaceId;
                doctor.PositionId = dto.PositionId;
                doctor.UserId = dto.UserId;

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
            var doctor = await _db.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            try
            {
                _db.Doctors.Remove(doctor);
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
