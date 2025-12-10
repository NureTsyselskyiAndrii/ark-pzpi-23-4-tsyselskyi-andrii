using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.LicenseNumber)
                   .IsRequired()
                   .HasMaxLength(511);

            builder.Property(d => d.UserId)
                   .IsRequired();
            builder.HasIndex(d => d.UserId, "IX_Doctors_UserId")
                   .IsUnique();
            builder.HasOne(d => d.User)
                   .WithOne(u => u.Doctor)
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(d => d.SpecializationId)
                   .IsRequired();
            builder.HasOne(d => d.Specialization)
                   .WithMany(s => s.Doctors)
                   .HasForeignKey(d => d.SpecializationId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.Property(d => d.WorkplaceId)
                   .IsRequired();
            builder.HasOne(d => d.Workplace)
                   .WithMany(w => w.Doctors)
                   .HasForeignKey(d => d.WorkplaceId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.Property(d => d.PositionId)
                   .IsRequired();
            builder.HasOne(d => d.Position)
                   .WithMany(p => p.Doctors)
                   .HasForeignKey(d => d.PositionId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    UserId = 2,
                    LicenseNumber = "LIC-2024-001",
                    SpecializationId = 1,
                    WorkplaceId = 1,
                    PositionId = 1
                },
                new Doctor
                {
                    Id = 2,
                    UserId = 3,
                    LicenseNumber = "LIC-2024-002",
                    SpecializationId = 3,
                    WorkplaceId = 1,
                    PositionId = 3
                }
            };
            builder.HasData(doctors);
        }
    }
}
