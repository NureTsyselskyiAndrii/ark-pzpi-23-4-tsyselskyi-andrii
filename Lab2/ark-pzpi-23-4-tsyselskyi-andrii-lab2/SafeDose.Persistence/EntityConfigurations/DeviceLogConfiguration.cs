using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class DeviceLogConfiguration : IEntityTypeConfiguration<DeviceLog>
    {
        public void Configure(EntityTypeBuilder<DeviceLog> builder)
        {
            builder.HasKey(dl => dl.Id);

            builder.Property(dl => dl.Description)
                   .IsRequired()
                   .HasMaxLength(4095);

            builder.Property(dl => dl.DeviceId)
                   .IsRequired();
            builder.HasOne(dl => dl.Device)
                   .WithMany(d => d.DeviceLogs)
                   .HasForeignKey(dl => dl.DeviceId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();
        }
    }
}
