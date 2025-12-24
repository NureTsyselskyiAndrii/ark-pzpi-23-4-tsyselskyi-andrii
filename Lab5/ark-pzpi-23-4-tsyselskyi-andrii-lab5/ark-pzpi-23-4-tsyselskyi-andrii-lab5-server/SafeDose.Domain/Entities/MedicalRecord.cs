using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class MedicalRecord : BaseEntity<long>
    {
        public string Description { get; set; } = null!;

        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
    }
}
