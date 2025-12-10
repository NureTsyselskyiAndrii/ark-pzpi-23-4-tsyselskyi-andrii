using SafeDose.Application.Contracts.Persistence;
using SafeDose.Domain.Entities;

namespace SafeDose.Application.Contracts.Persistance
{
    public interface IDispenseEventRepository : IGenericRepository<DispenseEvent, long>
    {
        Task DeleteByPatientIdAsync(long patientId);
    }
}
