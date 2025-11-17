using System.Numerics;

namespace Medicare.Domain.Models
{
    public class DoctorType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Doctor> Doctors { get; set; } = [];
    }
}
