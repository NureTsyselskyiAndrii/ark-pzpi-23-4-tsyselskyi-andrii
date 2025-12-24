using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SafeDose.Application.Contracts.DbContext;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.DbContexts
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private IDbContextTransaction _currentTransaction;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Workplace> Workplaces { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<DispenseEvent> DispenseEvents { get; set; }
        public DbSet<MedicationStock> MedicationStocks { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceLog> DeviceLogs { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Form> Forms { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public TEntity CreateProxy<TEntity>() where TEntity : class
        {
            return ProxiesExtensions.CreateProxy(this, typeof(TEntity)) as TEntity
                   ?? throw new InvalidOperationException($"Could not create proxy for type {typeof(TEntity).Name}");
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly,
                t => t.Namespace == "SafeDose.Persistence.EntityConfigurations");

            modelBuilder.HasDefaultSchema("business_schema");

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                       .SelectMany(t => t.GetProperties())
                       .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

            //var users = new List<User>
            //{
            //    new User
            //    {
            //        Id = 1L,
            //        FirstName = "Andrey",
            //        LastName = "Tsyselskyi",
            //        UserName = "Admin",
            //        Email = "tsyselskyiandrey@gmail.com",
            //        PhoneNumber = "+380682488040",
            //        CreatedDate = DateTime.UtcNow,
            //        Biography = "System administrator",
            //        BirthDate = new DateTime(1997, 01, 01)
            //    },
            //    new User
            //    {
            //        Id = 2L,
            //        FirstName = "John",
            //        LastName = "Doe",
            //        UserName = "JohnDoe",
            //        Email = "john.doe@example.com",
            //        PhoneNumber = "+380501112233",
            //        CreatedDate = DateTime.UtcNow,
            //        Biography = "Experienced surgeon",
            //        BirthDate = new DateTime(1990, 05, 10)
            //    },
            //    new User
            //    {
            //        Id = 3L,
            //        FirstName = "Anna",
            //        LastName = "Smith",
            //        UserName = "AnnaSmith",
            //        Email = "anna.smith@example.com",
            //        PhoneNumber = "+380631234567",
            //        CreatedDate = DateTime.UtcNow,
            //        Biography = "Caring nurse",
            //        BirthDate = new DateTime(1995, 10, 15)
            //    },
            //    new User
            //    {
            //        Id = 4L,
            //        FirstName = "Michael",
            //        LastName = "Brown",
            //        UserName = "MikeBrown",
            //        Email = "michael.brown@example.com",
            //        PhoneNumber = "+380973456789",
            //        CreatedDate = DateTime.UtcNow,
            //        Biography = "Fitness fan & healthy lifestyle supporter",
            //        BirthDate = new DateTime(1988, 03, 22)
            //    },
            //    new User
            //    {
            //        Id = 5L,
            //        FirstName = "Kate",
            //        LastName = "Jordan",
            //        UserName = "KateJordan",
            //        Email = "kate.jordan@example.com",
            //        PhoneNumber = "+380992223344",
            //        CreatedDate = DateTime.UtcNow,
            //        Biography = "Designer & creative thinker",
            //        BirthDate = new DateTime(1999, 12, 01)
            //    }
            //};
            //modelBuilder.Entity<User>().HasData(users);

            //var workplaces = new List<Workplace>
            //{
            //    new Workplace { Id = 1, Name = "City Hospital #1", Address = "123 Main St, Kyiv" },
            //    new Workplace { Id = 2, Name = "Private Clinic 'HealthPlus'", Address = "45 Green Ave, Lviv" },
            //    new Workplace { Id = 3, Name = "Regional Medical Center", Address = "81 Central Rd, Odesa" },
            //    new Workplace { Id = 4, Name = "Family Clinic 'DobroMed'", Address = "14 Peace St, Dnipro" },
            //    new Workplace { Id = 5, Name = "Diagnostic Center 'MedExpert'", Address = "27 Horizon Blvd, Kharkiv" }
            //};
            //modelBuilder.Entity<Workplace>().HasData(workplaces);

            //var specializations = new List<Specialization>
            //{
            //    new Specialization { Id = 1, Name = "Cardiologist" },
            //    new Specialization { Id = 2, Name = "Neurologist" },
            //    new Specialization { Id = 3, Name = "Dermatologist" },
            //    new Specialization { Id = 4, Name = "Pediatrician" },
            //    new Specialization { Id = 5, Name = "General Physician" }
            //};
            //modelBuilder.Entity<Specialization>().HasData(specializations);

            //var positions = new List<Position>
            //{
            //    new Position { Id = 1, Name = "Senior Doctor" },
            //    new Position { Id = 2, Name = "Junior Doctor" },
            //    new Position { Id = 3, Name = "Consultant" },
            //    new Position { Id = 4, Name = "Intern" },
            //    new Position { Id = 5, Name = "Head of Department" }
            //};
            //modelBuilder.Entity<Position>().HasData(positions);

            //var patients = new List<Patient>
            //{
            //    new Patient
            //    {
            //        Id = 1,
            //        UserId = 4,
            //        BloodType = "A+"
            //    },
            //    new Patient
            //    {
            //        Id = 2,
            //        UserId = 5,
            //        BloodType = "O-"
            //    }
            //};
            //modelBuilder.Entity<Patient>().HasData(patients);

            //var doctors = new List<Doctor>
            //{
            //    new Doctor
            //    {
            //        Id = 1,
            //        UserId = 2,
            //        LicenseNumber = "LIC-2024-001",
            //        SpecializationId = 1,
            //        WorkplaceId = 1,
            //        PositionId = 1
            //    },
            //    new Doctor
            //    {
            //        Id = 2,
            //        UserId = 3,
            //        LicenseNumber = "LIC-2024-002",
            //        SpecializationId = 3,
            //        WorkplaceId = 1,
            //        PositionId = 3
            //    }
            //};
            //modelBuilder.Entity<Doctor>().HasData(doctors);

            //var forms = new List<Form>
            //{
            //    new Form { Id = 1, Name = "Tablet" },
            //    new Form { Id = 2, Name = "Capsule" },
            //    new Form { Id = 3, Name = "Syrup" },
            //    new Form { Id = 4, Name = "Injection" },
            //    new Form { Id = 5, Name = "Ointment" }
            //};
            //modelBuilder.Entity<Form>().HasData(forms);

            //var manufacturers = new List<Manufacturer>
            //{
            //    new Manufacturer { Id = 1, Name = "Pfizer" },
            //    new Manufacturer { Id = 2, Name = "Moderna" },
            //    new Manufacturer { Id = 3, Name = "Bayer" },
            //    new Manufacturer { Id = 4, Name = "Novartis" },
            //    new Manufacturer { Id = 5, Name = "Johnson & Johnson" }
            //};
            //modelBuilder.Entity<Manufacturer>().HasData(manufacturers);
        }
    }
}
