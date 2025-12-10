using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Workplace : BaseEntity<long>
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
        public virtual ICollection<MedicationStock> MedicationStocks { get; set; } = new List<MedicationStock>();
    }
}
