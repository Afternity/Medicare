namespace Medicare.ClientWPF.DTOs
{
    public class RecipeDto
    {
        public Guid Id { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public string Medication { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public TimeSpan DurationDays { get; set; } = TimeSpan.Zero;
    }
}
