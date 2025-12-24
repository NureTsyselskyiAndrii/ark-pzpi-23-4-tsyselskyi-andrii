using Microsoft.EntityFrameworkCore;
using SafeDose.Application.Contracts.Persistance;
using SafeDose.Domain.Entities;
using SafeDose.Persistence.DbContexts;

namespace SafeDose.Persistence.Repositories
{
    public class PrescriptionRepository : GenericRepository<Prescription, long>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task DeleteByPatientIdAsync(long patientId)
        {
            var ids = await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var prescriptionId in ids)
                await DeleteAsync(prescriptionId);
        }

        public override async Task DeleteAsync(long id)
        {
            await _context.DispenseEvents
                .Where(de => de.PrescriptionId == id)
                .ExecuteDeleteAsync();

            await _context.Prescriptions
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
