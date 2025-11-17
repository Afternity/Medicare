namespace Medicare.Domain.Models
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Guid DoctorTypeId { get; set; }
        public virtual DoctorType DoctorType { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = [];
        public virtual ICollection<Recipe> Recipes { get; set; } = [];
    }
}
