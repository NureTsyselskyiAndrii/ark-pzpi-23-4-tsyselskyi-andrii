using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Device : BaseEntity<long>
    {
        public string Name { get; set; } = null!;

        public long WorkplaceId { get; set; }
        public virtual Workplace Workplace { get; set; } = null!;

        public virtual ICollection<DeviceLog> DeviceLogs { get; set; } = new List<DeviceLog>();
        public virtual ICollection<DispenseEvent> DispenseEvents { get; set; } = new List<DispenseEvent>();
    }
}
