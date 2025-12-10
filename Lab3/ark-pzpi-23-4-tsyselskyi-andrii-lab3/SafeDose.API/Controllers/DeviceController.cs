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
    public class DeviceController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public DeviceController(IApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _db.Devices
                .Include(d => d.Workplace)
                .ToListAsync();

            return Ok(devices.Select(d => new
            {
                d.Id,
                d.Name,
                Workplace = new
                {
                    d.Workplace.Id,
                    d.Workplace.Name
                }
            }));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var device = await _db.Devices
                .Include(d => d.Workplace)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound();

            return Ok(new
            {
                device.Id,
                device.Name,
                Workplace = new
                {
                    device.Workplace.Id,
                    device.Workplace.Name
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpdateDeviceRequest dto)
        {
            try
            {
                _db.Devices.Add(new Device
                {
                    Name = dto.Name,
                    WorkplaceId = dto.WorkplaceId
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDeviceRequest dto)
        {
            var device = await _db.Devices.FindAsync(id);
            if (device == null)
                return NotFound();

            try
            {
                device.Name = dto.Name;
                device.WorkplaceId = dto.WorkplaceId;

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
            var device = await _db.Devices.FindAsync(id);
            if (device == null)
                return NotFound();

            try
            {
                _db.Devices.Remove(device);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }


            return NoContent();
        }

        [HttpGet("{deviceId}/logs")]
        public async Task<IActionResult> GetLogs(long deviceId)
        {
            var logs = await _db.DeviceLogs
                .Where(l => l.DeviceId == deviceId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return Ok(logs.Select(dl => new
            {
                dl.Id,
                dl.Description,
                dl.CreatedAt
            }));
        }

        [HttpPost("{deviceId}/logs")]
        public async Task<IActionResult> AddLog(long deviceId, [FromBody] AddLogRequest dto)
        {
            var device = await _db.Devices.FindAsync(deviceId);
            if (device == null)
                return NotFound("Device not found");

            try
            {
                var log = new DeviceLog
                {
                    DeviceId = deviceId,
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _db.DeviceLogs.Add(log);
                await _db.SaveChangesAsync();
                return Ok(log);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }

        [HttpDelete("logs/{logId}")]
        public async Task<IActionResult> DeleteLog(long logId)
        {
            var log = await _db.DeviceLogs.FindAsync(logId);
            if (log == null)
                return NotFound();

            try
            {
                _db.DeviceLogs.Remove(log);
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
