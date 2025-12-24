using SafeDose.Application.Contracts.Persistance;
using SafeDose.Domain.Entities;
using SafeDose.Persistence.DbContexts;

namespace SafeDose.Persistence.Repositories
{
    public class PatientRepository : GenericRepository<Patient, long>, IPatientRepository
    {
        private readonly IDispenseEventRepository _dispenseEventRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PatientRepository(ApplicationDbContext context, IDispenseEventRepository dispenseEventRepository, IPrescriptionRepository prescriptionRepository) : base(context)
        {
            _dispenseEventRepository = dispenseEventRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        public override async Task DeleteAsync(long id)
        {
            await _dispenseEventRepository.DeleteByPatientIdAsync(id);
            await _prescriptionRepository.DeleteByPatientIdAsync(id);

            await base.DeleteAsync(id);
        }
    }
}
