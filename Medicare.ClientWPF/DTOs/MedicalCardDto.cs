namespace Medicare.ClientWPF.DTOs
{
    public class MedicalCardDto
    {
        public Guid Id { get; set; }
        public string MedicalHistory { get; set; } = string.Empty;
        public string Allergies {  get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
    }
}
