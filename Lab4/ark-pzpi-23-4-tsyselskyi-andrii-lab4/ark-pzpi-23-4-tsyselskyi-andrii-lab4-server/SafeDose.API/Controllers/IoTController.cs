using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeDose.Domain.Entities;
using SafeDose.Persistence.DbContexts;

namespace SafeDose.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IoTController : ControllerBase
    {
        private readonly ILogger<IoTController> _logger;
        private readonly ApplicationDbContext _context;

        public IoTController(ILogger<IoTController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] ScanRequest request)
        {
            try
            {
                var device = await _context.Devices
                    .Include(d => d.Workplace)
                    .FirstOrDefaultAsync(d => d.Name == request.DeviceId);

                if (device == null)
                {
                    return Ok(new { allowed = false, reason = "Device not found" });
                }

                var prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Doctor)
                    .Include(p => p.PrescriptionMedications)
                    .ThenInclude(pm => pm.Medication)
                    .Where(p => p.DoctorId == request.UserId &&
                               p.StartDate <= DateTime.Now &&
                               p.EndDate >= DateTime.Now)
                    .FirstOrDefaultAsync();

                if (prescription == null)
                {
                    return Ok(new { allowed = false, reason = "No valid prescription found" });
                }

                var medication = prescription.PrescriptionMedications
                    .FirstOrDefault(pm => pm.MedicationId == request.MedicineId);

                if (medication == null)
                {
                    return Ok(new { allowed = false, reason = "Medication not in prescription" });
                }

                var log = new DeviceLog
                {
                    DeviceId = device.Id,
                    Description = $"Scan authorized for patient {prescription.Patient.User.FirstName} {prescription.Patient.User.LastName}",
                    CreatedAt = DateTime.Now
                };
                _context.DeviceLogs.Add(log);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    allowed = true,
                    prescriptionId = prescription.Id,
                    patientId = prescription.PatientId,
                    doctorId = prescription.DoctorId,
                    medicationId = medication.MedicationId,
                    dosage = medication.Dosage,
                    patientName = $"{prescription.Patient.User.FirstName} {prescription.Patient.User.LastName}",
                    medicationName = medication.Medication.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scan authorization");
                return Ok(new { allowed = false, reason = "System error" });
            }
        }

        [HttpPost("dispense")]
        public async Task<IActionResult> RecordDispense([FromBody] DispenseRequest request)
        {
            try
            {
                var device = await _context.Devices
                    .FirstOrDefaultAsync(d => d.Name == request.DeviceId);

                if (device == null)
                {
                    return BadRequest("Device not found");
                }

                var dispenseEvent = new DispenseEvent
                {
                    PrescriptionId = request.PrescriptionId,
                    DeviceId = device.Id,
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    MedicationId = request.MedicationId,
                    QuantityDispensed = request.QuantityDispensed,
                    DispensedAt = DateTime.Now,
                    Price = await CalculatePrice(request.MedicationId, request.QuantityDispensed)
                };

                _context.DispenseEvents.Add(dispenseEvent);

                var log = new DeviceLog
                {
                    DeviceId = device.Id,
                    Description = $"Dispensed {request.QuantityDispensed} units of medication ID {request.MedicationId}",
                    CreatedAt = DateTime.Now
                };
                _context.DeviceLogs.Add(log);

                await _context.SaveChangesAsync();

                return Ok(new { success = true, dispenseEventId = dispenseEvent.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording dispense event");
                return StatusCode(500, "Error recording dispense");
            }
        }

        [HttpPost("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusRequest request)
        {
            try
            {
                var device = await _context.Devices
                    .FirstOrDefaultAsync(d => d.Name == request.DeviceId);

                if (device == null)
                {
                    return BadRequest("Device not found");
                }

                var log = new DeviceLog
                {
                    DeviceId = device.Id,
                    Description = $"Status: {request.Status}, Temp: {request.Temperature}°C, Humidity: {request.Humidity}%, Inventory: {request.InventoryCount}",
                    CreatedAt = DateTime.Now
                };
                _context.DeviceLogs.Add(log);

                if (request.Status == "emergency")
                {
                    await HandleEmergency(device.Id, request.DeviceId);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Device {request.DeviceId} status: {request.Status}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device status");
                return StatusCode(500, "Error updating status");
            }
        }

        [HttpPost("log")]
        public async Task<IActionResult> AddLog([FromBody] LogRequest request)
        {
            try
            {
                var device = await _context.Devices
                    .FirstOrDefaultAsync(d => d.Name == request.DeviceId);

                if (device == null)
                {
                    return BadRequest("Device not found");
                }

                var log = new DeviceLog
                {
                    DeviceId = device.Id,
                    Description = request.Description,
                    CreatedAt = DateTime.Now
                };

                _context.DeviceLogs.Add(log);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding device log");
                return StatusCode(500, "Error adding log");
            }
        }

        [HttpPost("inventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] InventoryRequest request)
        {
            try
            {
                var device = await _context.Devices
                    .FirstOrDefaultAsync(d => d.Name == request.DeviceId);

                if (device == null)
                {
                    return BadRequest("Device not found");
                }

                var log = new DeviceLog
                {
                    DeviceId = device.Id,
                    Description = $"Inventory update - Medication ID: {request.MedicationId}, Count: {request.CurrentCount}",
                    CreatedAt = DateTime.Now
                };
                _context.DeviceLogs.Add(log);

                if (request.CurrentCount <= 5)
                {
                    await HandleLowInventory(device.Id, request.MedicationId, request.CurrentCount);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory");
                return StatusCode(500, "Error updating inventory");
            }
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                var devices = await _context.Devices
                    .Include(d => d.Workplace)
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        WorkplaceName = d.Workplace.Name,
                        LastActivity = d.DeviceLogs.OrderByDescending(l => l.CreatedAt).FirstOrDefault().CreatedAt
                    })
                    .ToListAsync();

                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting devices");
                return StatusCode(500, "Error retrieving devices");
            }
        }

        [HttpGet("device/{deviceId}/logs")]
        public async Task<IActionResult> GetDeviceLogs(string deviceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var device = await _context.Devices
                    .FirstOrDefaultAsync(d => d.Name == deviceId);

                if (device == null)
                {
                    return NotFound("Device not found");
                }

                var logs = await _context.DeviceLogs
                    .Where(l => l.DeviceId == device.Id)
                    .OrderByDescending(l => l.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new
                    {
                        l.Id,
                        l.Description,
                        l.CreatedAt
                    })
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device logs");
                return StatusCode(500, "Error retrieving logs");
            }
        }

        [HttpGet("analytics/dispense")]
        public async Task<IActionResult> GetDispenseAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddDays(-30);
                var end = endDate ?? DateTime.Now;

                var analytics = await _context.DispenseEvents
                    .Where(de => de.DispensedAt >= start && de.DispensedAt <= end)
                    .GroupBy(de => new { de.DeviceId, de.MedicationId })
                    .Select(g => new
                    {
                        DeviceId = g.Key.DeviceId,
                        MedicationId = g.Key.MedicationId,
                        TotalDispensed = g.Sum(de => de.QuantityDispensed),
                        TotalRevenue = g.Sum(de => de.Price),
                        DispenseCount = g.Count()
                    })
                    .ToListAsync();

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dispense analytics");
                return StatusCode(500, "Error retrieving analytics");
            }
        }

        private async Task<decimal> CalculatePrice(long medicationId, int quantity)
        {
            var medication = await _context.Medications.FindAsync(medicationId);
            return medication?.PricePerBlister * quantity ?? 0;
        }

        private async Task HandleEmergency(long deviceId, string deviceName)
        {
            var emergencyLog = new DeviceLog
            {
                DeviceId = deviceId,
                Description = "EMERGENCY ALERT - Immediate attention required",
                CreatedAt = DateTime.Now
            };
            _context.DeviceLogs.Add(emergencyLog);
            _logger.LogWarning($"EMERGENCY ALERT from device {deviceName}");
        }

        private async Task HandleLowInventory(long deviceId, int medicationId, int currentCount)
        {
            var inventoryLog = new DeviceLog
            {
                DeviceId = deviceId,
                Description = $"LOW INVENTORY ALERT - Medication ID {medicationId} has only {currentCount} units remaining",
                CreatedAt = DateTime.Now
            };
            _context.DeviceLogs.Add(inventoryLog);

            _logger.LogWarning($"Low inventory alert - Device {deviceId}, Medication {medicationId}, Count: {currentCount}");
        }
    }
    public class ScanRequest
    {
        public string DeviceId { get; set; }
        public int MedicineId { get; set; }
        public int UserId { get; set; }
        public long WorkplaceId { get; set; }
    }

    public class StatusRequest
    {
        public string DeviceId { get; set; }
        public string Status { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public int InventoryCount { get; set; }
        public long WorkplaceId { get; set; }
        public long Uptime { get; set; }
    }

    public class DispenseRequest
    {
        public string DeviceId { get; set; }
        public long PrescriptionId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public long MedicationId { get; set; }
        public int QuantityDispensed { get; set; }
        public float Temperature { get; set; }
        public int InventoryCount { get; set; }
    }

    public class LogRequest
    {
        public string DeviceId { get; set; }
        public string Description { get; set; }
    }

    public class InventoryRequest
    {
        public string DeviceId { get; set; }
        public int MedicationId { get; set; }
        public int CurrentCount { get; set; }
    }
}
