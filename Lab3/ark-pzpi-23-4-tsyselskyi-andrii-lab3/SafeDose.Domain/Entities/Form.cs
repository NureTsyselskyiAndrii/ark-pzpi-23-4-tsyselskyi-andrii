using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Form : BaseEntity<long>
    {
        public string Name { get; set; } = null!;

        public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
    }
}
