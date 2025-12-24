using Microsoft.EntityFrameworkCore;
using SafeDose.Domain.Entities;

namespace SafeDose.Application.Contracts.DbContext
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Doctor> Doctors { get; set; }
        DbSet<Patient> Patients { get; set; }
        DbSet<Specialization> Specializations { get; set; }
        DbSet<Workplace> Workplaces { get; set; }
        DbSet<Position> Positions { get; set; }
        DbSet<MedicalRecord> MedicalRecords { get; set; }
        DbSet<DispenseEvent> DispenseEvents { get; set; }
        DbSet<MedicationStock> MedicationStocks { get; set; }
        DbSet<Device> Devices { get; set; }
        DbSet<DeviceLog> DeviceLogs { get; set; }
        DbSet<Prescription> Prescriptions { get; set; }
        DbSet<Medication> Medications { get; set; }
        DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }
        DbSet<Manufacturer> Manufacturers { get; set; }
        DbSet<Form> Forms { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        TEntity CreateProxy<TEntity>() where TEntity : class;
    }
}
