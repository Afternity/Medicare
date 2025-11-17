namespace Medicare.Domain.Models
{
    public class MedicalCard
    {
        public Guid Id { get; set; }
        public string MedicalHistory { get; set; } = string.Empty;
        public string Allergies { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;

        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;
    }
}
