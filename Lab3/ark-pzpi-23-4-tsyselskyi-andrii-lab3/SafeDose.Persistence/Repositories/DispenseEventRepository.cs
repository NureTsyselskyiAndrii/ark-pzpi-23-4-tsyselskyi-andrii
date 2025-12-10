using Microsoft.EntityFrameworkCore;
using SafeDose.Application.Contracts.Persistance;
using SafeDose.Domain.Entities;
using SafeDose.Persistence.DbContexts;

namespace SafeDose.Persistence.Repositories
{
    public class DispenseEventRepository : GenericRepository<DispenseEvent, long>, IDispenseEventRepository
    {
        public DispenseEventRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task DeleteByPatientIdAsync(long patientId)
        {
            await _context.DispenseEvents
                .Where(de => de.PatientId == patientId)
                .ExecuteDeleteAsync();
        }
    }
}
