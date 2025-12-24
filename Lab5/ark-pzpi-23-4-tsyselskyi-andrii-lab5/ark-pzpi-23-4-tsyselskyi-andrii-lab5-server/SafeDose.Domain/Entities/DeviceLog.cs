using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class DeviceLog : BaseEntity<long>
    {
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public long DeviceId { get; set; }
        public virtual Device Device { get; set; } = null!;
    }
}
