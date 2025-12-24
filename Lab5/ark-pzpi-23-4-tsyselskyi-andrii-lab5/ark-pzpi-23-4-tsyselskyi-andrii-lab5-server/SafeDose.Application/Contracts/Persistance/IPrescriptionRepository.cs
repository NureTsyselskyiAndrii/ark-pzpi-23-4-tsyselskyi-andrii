using SafeDose.Application.Contracts.Persistence;
using SafeDose.Domain.Entities;

namespace SafeDose.Application.Contracts.Persistance
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription, long>
    {
        Task DeleteByPatientIdAsync(long patientId);
    }
}
