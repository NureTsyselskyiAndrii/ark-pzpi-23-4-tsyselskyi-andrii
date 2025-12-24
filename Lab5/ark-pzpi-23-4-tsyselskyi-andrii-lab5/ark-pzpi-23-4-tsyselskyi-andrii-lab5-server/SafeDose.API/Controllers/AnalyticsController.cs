using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeDose.Application.Contracts.DbContext;

namespace SafeDose.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public AnalyticsController(IApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet("dispense/total")]
        public async Task<IActionResult> GetTotalDispensed([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = _db.DispenseEvents.AsQueryable();

            if (from.HasValue) query = query.Where(x => x.DispensedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.DispensedAt <= to.Value);

            var total = await query.SumAsync(x => x.Price * x.QuantityDispensed);

            return Ok(new { total });
        }

        [HttpGet("dispense/by-medication")]
        public async Task<IActionResult> GetDispenseByMedication()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => x.Medication.Name)
                .Select(g => new
                {
                    Medication = g.Key,
                    TotalQuantity = g.Sum(e => e.QuantityDispensed),
                    TotalRevenue = g.Sum(e => e.Price * e.QuantityDispensed)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("dispense/by-doctor")]
        public async Task<IActionResult> GetDispenseByDoctor()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => new { x.DoctorId, x.Doctor.User.FirstName, x.Doctor.User.LastName })
                .Select(g => new
                {
                    DoctorId = g.Key.DoctorId,
                    DoctorName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalEvents = g.Count(),
                    TotalRevenue = g.Sum(e => e.Price * e.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("dispense/by-patient")]
        public async Task<IActionResult> GetDispenseByPatient()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => new { x.PatientId, x.Patient.User.FirstName, x.Patient.User.LastName })
                .Select(g => new
                {
                    PatientId = g.Key.PatientId,
                    PatientName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalEvents = g.Count(),
                    TotalSpent = g.Sum(e => e.Price * e.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("dispense/by-device")]
        public async Task<IActionResult> GetDispenseByDevice()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => x.Device.Name)
                .Select(g => new
                {
                    Device = g.Key,
                    TotalDispenseEvents = g.Count(),
                    TotalRevenue = g.Sum(e => e.Price * e.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("dispense/time-distribution")]
        public async Task<IActionResult> GetDispenseTimeDistribution()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => x.DispensedAt.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Hour)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("revenue/total")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var revenue = await _db.DispenseEvents
                .SumAsync(x => x.Price * x.QuantityDispensed);

            return Ok(new { revenue });
        }

        [HttpGet("revenue/by-workplace")]
        public async Task<IActionResult> GetRevenueByWorkplace()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => x.Device.Workplace.Name)
                .Select(g => new
                {
                    Workplace = g.Key,
                    TotalRevenue = g.Sum(e => e.Price * e.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("revenue/by-medication")]
        public async Task<IActionResult> GetRevenueByMedication()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => x.Medication.Name)
                .Select(g => new
                {
                    Medication = g.Key,
                    Revenue = g.Sum(e => e.Price * e.QuantityDispensed)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("revenue/forecast")]
        public async Task<IActionResult> GetRevenueForecast()
        {
            var last30 = DateTime.UtcNow.AddDays(-30);

            var revenue = await _db.DispenseEvents
                .Where(x => x.DispensedAt >= last30)
                .GroupBy(x => x.DispensedAt.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(e => e.Price * e.QuantityDispensed) })
                .ToListAsync();

            if (!revenue.Any()) return Ok(new { forecast = 0 });

            var avgDaily = revenue.Average(x => x.Revenue);

            var forecast = avgDaily * 30; // прогноз на 30 днів

            return Ok(new { forecast, avgDaily });
        }

        [HttpGet("stock/low")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            var data = await _db.MedicationStocks
                .GroupBy(x => x.Medication.Name)
                .Select(g => new
                {
                    Medication = g.Key,
                    Total = g.Sum(x => x.Quantity)
                })
                .Where(x => x.Total <= threshold)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("stock/expiration/soon")]
        public async Task<IActionResult> GetExpirationSoon([FromQuery] int days = 30)
        {
            var limit = DateTime.UtcNow.AddDays(days);

            var data = await _db.MedicationStocks
                .Where(x => x.ExpirationDate <= limit)
                .Select(x => new
                {
                    x.Medication.Name,
                    x.Quantity,
                    x.ExpirationDate
                })
                .OrderBy(x => x.ExpirationDate)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("stock/turnover")]
        public async Task<IActionResult> GetStockTurnover()
        {
            var sold = await _db.DispenseEvents
                .GroupBy(x => x.MedicationId)
                .Select(g => new { MedicationId = g.Key, Sold = g.Sum(x => x.QuantityDispensed) })
                .ToListAsync();

            var stock = await _db.MedicationStocks
                .GroupBy(x => x.MedicationId)
                .Select(g => new { MedicationId = g.Key, Stock = g.Sum(x => x.Quantity) })
                .ToListAsync();

            var result = from s in sold
                         join st in stock on s.MedicationId equals st.MedicationId
                         select new
                         {
                             MedicationId = s.MedicationId,
                             Turnover = st.Stock == 0 ? 0 : (decimal)s.Sold / st.Stock
                         };

            return Ok(result);
        }

        [HttpGet("stock/demand-prediction")]
        public async Task<IActionResult> GetDemandPrediction()
        {
            var date = DateTime.UtcNow.AddDays(-14);

            var data = await _db.DispenseEvents
                .Where(x => x.DispensedAt >= date)
                .GroupBy(x => x.Medication.Name)
                .Select(g => new
                {
                    Medication = g.Key,
                    AvgDaily = g.Sum(x => x.QuantityDispensed) / 14m,
                    Forecast14Days = g.Sum(x => x.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("doctor/prescriptions-count")]
        public async Task<IActionResult> GetDoctorPrescriptionCount()
        {
            var data = await _db.Prescriptions
                .GroupBy(p => new { p.DoctorId, p.Doctor.User.FirstName, p.Doctor.User.LastName })
                .Select(g => new
                {
                    DoctorId = g.Key.DoctorId,
                    Doctor = g.Key.FirstName + " " + g.Key.LastName,
                    PrescriptionsIssued = g.Count()
                })
                .OrderByDescending(x => x.PrescriptionsIssued)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("doctor/dispense-performance")]
        public async Task<IActionResult> GetDoctorDispensePerformance()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => new { x.DoctorId, x.Doctor.User.FirstName, x.Doctor.User.LastName })
                .Select(g => new
                {
                    DoctorId = g.Key.DoctorId,
                    Doctor = g.Key.FirstName + " " + g.Key.LastName,
                    TotalEvents = g.Count(),
                    TotalQuantity = g.Sum(x => x.QuantityDispensed),
                    TotalRevenue = g.Sum(x => x.Price * x.QuantityDispensed)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("doctor/avg-revenue")]
        public async Task<IActionResult> GetDoctorAvgRevenue()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => new { x.DoctorId, x.Doctor.User.FirstName, x.Doctor.User.LastName })
                .Select(g => new
                {
                    Doctor = g.Key.FirstName + " " + g.Key.LastName,
                    AvgRevenue = g.Average(x => x.Price * x.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("patient/activity")]
        public async Task<IActionResult> GetPatientActivity()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => new { x.PatientId, x.Patient.User.FirstName, x.Patient.User.LastName })
                .Select(g => new
                {
                    PatientId = g.Key.PatientId,
                    Patient = g.Key.FirstName + " " + g.Key.LastName,
                    DispenseEvents = g.Count()
                })
                .OrderByDescending(x => x.DispenseEvents)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("patient/medication-history/{id}")]
        public async Task<IActionResult> GetPatientMedicationHistory(long id)
        {
            var data = await _db.DispenseEvents
                .Where(x => x.PatientId == id)
                .GroupBy(x => x.Medication.Name)
                .Select(g => new
                {
                    Medication = g.Key,
                    TimesDispensed = g.Count(),
                    TotalQuantity = g.Sum(x => x.QuantityDispensed)
                })
                .OrderByDescending(x => x.TimesDispensed)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("patient/total-spent/{id}")]
        public async Task<IActionResult> GetPatientTotalSpent(long id)
        {
            var spent = await _db.DispenseEvents
                .Where(x => x.PatientId == id)
                .SumAsync(x => x.Price * x.QuantityDispensed);

            return Ok(new { PatientId = id, TotalSpent = spent });
        }

        [HttpGet("patient/visits/{id}")]
        public async Task<IActionResult> GetPatientVisits(long id)
        {
            var data = await _db.DispenseEvents
                .Where(x => x.PatientId == id)
                .GroupBy(x => x.DispensedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("prescriptions/common-medications")]
        public async Task<IActionResult> GetCommonMedications()
        {
            var data = await _db.PrescriptionMedications
                .GroupBy(x => x.Medication.Name)
                .Select(g => new
                {
                    Medication = g.Key,
                    Count = g.Count(),
                    TotalDosages = g.Sum(x => x.QuantityOfDosagesOverall)
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("prescriptions/expired")]
        public async Task<IActionResult> GetExpiredPrescriptions()
        {
            var now = DateTime.UtcNow;

            var data = await _db.Prescriptions
                .Where(x => x.EndDate < now)
                .Select(x => new
                {
                    x.Id,
                    Patient = x.Patient.User.FirstName + " " + x.Patient.User.LastName,
                    Doctor = x.Doctor.User.FirstName + " " + x.Doctor.User.LastName,
                    x.StartDate,
                    x.EndDate
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("prescriptions/avg-duration")]
        public async Task<IActionResult> GetAveragePrescriptionDuration()
        {
            var data = await _db.Prescriptions
                .Select(x => EF.Functions.DateDiffDay(x.StartDate, x.EndDate))
                .ToListAsync();

            var avg = data.Any() ? data.Average() : 0;

            return Ok(new { AverageDays = avg });
        }

        [HttpGet("prescriptions/by-doctor/{id}")]
        public async Task<IActionResult> GetPrescriptionsByDoctor(long id)
        {
            var data = await _db.Prescriptions
                .Where(x => x.DoctorId == id)
                .Select(x => new
                {
                    x.Id,
                    x.PatientId,
                    Patient = x.Patient.User.FirstName + " " + x.Patient.User.LastName,
                    x.StartDate,
                    x.EndDate
                })
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("device/log-frequency")]
        public async Task<IActionResult> GetDeviceLogFrequency()
        {
            var data = await _db.DeviceLogs
                .GroupBy(x => x.Device.Name)
                .Select(g => new
                {
                    Device = g.Key,
                    Logs = g.Count()
                })
                .OrderByDescending(x => x.Logs)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("device/activity")]
        public async Task<IActionResult> GetDeviceActivity()
        {
            var data = await _db.DispenseEvents
                .GroupBy(x => x.Device.Name)
                .Select(g => new
                {
                    Device = g.Key,
                    Events = g.Count(),
                    TotalQuantity = g.Sum(x => x.QuantityDispensed)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("device/errors")]
        public async Task<IActionResult> GetDeviceErrors()
        {
            var data = await _db.DeviceLogs
                .Where(x => x.Description.Contains("error") ||
                            x.Description.Contains("fail") ||
                            x.Description.Contains("exception"))
                .GroupBy(x => new { x.Device.Name, x.Description })
                .Select(g => new
                {
                    Device = g.Key.Name,
                    Error = g.Key.Description,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return Ok(data);
        }

    }
}
