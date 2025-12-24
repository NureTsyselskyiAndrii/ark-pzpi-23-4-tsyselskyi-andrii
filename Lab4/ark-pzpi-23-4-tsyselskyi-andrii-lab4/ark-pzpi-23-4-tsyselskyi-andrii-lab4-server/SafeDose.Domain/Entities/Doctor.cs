using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Doctor : BaseEntity<long>
    {
        public string LicenseNumber { get; set; } = null!;

        public long SpecializationId { get; set; }
        public virtual Specialization Specialization { get; set; } = null!;

        public long WorkplaceId { get; set; }
        public virtual Workplace Workplace { get; set; } = null!;

        public long PositionId { get; set; }
        public virtual Position Position { get; set; } = null!;

        public long UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public virtual ICollection<DispenseEvent> DispenseEvents { get; set; } = new List<DispenseEvent>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
