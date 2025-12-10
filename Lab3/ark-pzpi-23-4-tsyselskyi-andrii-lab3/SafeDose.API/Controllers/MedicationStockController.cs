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
    public class MedicationStockController : ControllerBase
    {
        private readonly IApplicationDbContext _db;

        public MedicationStockController(IApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _db.MedicationStocks
                .Include(s => s.Workplace)
                .Include(s => s.Medication)
                .ToListAsync();

            return Ok(stocks.Select(s => new
            {
                s.Id,
                s.Quantity,
                s.ProductionDate,
                s.ExpirationDate,
                s.ReceivedAt,

                Workplace = new { s.Workplace.Id, s.Workplace.Name },
                Medication = new { s.Medication.Id, s.Medication.Name, s.Medication.Barcode }
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var s = await _db.MedicationStocks
                .Include(o => o.Workplace)
                .Include(o => o.Medication)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (s == null)
                return NotFound();

            return Ok(new
            {
                s.Id,
                s.Quantity,
                s.ProductionDate,
                s.ExpirationDate,
                s.ReceivedAt,

                Workplace = new { s.Workplace.Id, s.Workplace.Name },
                Medication = new { s.Medication.Id, s.Medication.Name, s.Medication.Barcode }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicationStockRequest dto)
        {
            try
            {
                var stock = new MedicationStock
                {
                    Quantity = dto.Quantity,
                    ProductionDate = dto.ProductionDate,
                    ExpirationDate = dto.ExpirationDate,
                    ReceivedAt = dto.ReceivedAt,
                    WorkplaceId = dto.WorkplaceId,
                    MedicationId = dto.MedicationId
                };

                _db.MedicationStocks.Add(stock);
                await _db.SaveChangesAsync();

                return Ok(stock.Id);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateMedicationStockRequest dto)
        {
            var stock = await _db.MedicationStocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.Quantity = dto.Quantity;
                stock.ProductionDate = dto.ProductionDate;
                stock.ExpirationDate = dto.ExpirationDate;
                stock.ReceivedAt = dto.ReceivedAt;
                stock.WorkplaceId = dto.WorkplaceId;
                stock.MedicationId = dto.MedicationId;

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
            var stock = await _db.MedicationStocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            try
            {
                _db.MedicationStocks.Remove(stock);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            return NoContent();
        }

        [HttpPost("{id}/add-quantity")]
        public async Task<IActionResult> AddQuantity(long id, [FromQuery] int amount)
        {
            if (amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var stock = await _db.MedicationStocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            try
            {
                stock.Quantity += amount;
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException();
            }

            return Ok(new { stock.Id, stock.Quantity });
        }

        [HttpPost("{id}/subtract-quantity")]
        public async Task<IActionResult> SubtractQuantity(long id, [FromQuery] int amount)
        {
            if (amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var stock = await _db.MedicationStocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            if (stock.Quantity < amount)
                return BadRequest($"Not enough stock. Current quantity: {stock.Quantity}");

            try
            {
                stock.Quantity -= amount;
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException();
            }

            return Ok(new { stock.Id, stock.Quantity });
        }
    }
}
