using SafeDose.Application.Contracts.Persistence;
using SafeDose.Domain.Entities;

namespace SafeDose.Application.Contracts.Persistance
{
    public interface IPatientRepository : IGenericRepository<Patient, long>
    {
    }
}
