namespace Medicare.Domain.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public string Medication { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public TimeSpan DurationDays { get; set; } = TimeSpan.Zero;

        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
