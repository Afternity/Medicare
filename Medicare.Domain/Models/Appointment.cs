namespace Medicare.Domain.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled
        public string Notes { get; set; } = string.Empty;

        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
