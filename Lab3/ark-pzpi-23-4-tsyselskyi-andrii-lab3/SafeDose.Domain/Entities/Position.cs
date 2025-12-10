using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Position : BaseEntity<long>
    {
        public string Name { get; set; } = null!;

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
