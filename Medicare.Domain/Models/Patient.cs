namespace Medicare.Domain.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<MedicalCard> MedicalCards { get; set; } = [];
        public virtual ICollection<Appointment> Appointments { get; set; } = [];
        public virtual ICollection<Recipe> Recipes { get; set; } = [];
    }
}
