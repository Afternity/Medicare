namespace Medicare.ClientWPF.DTOs
{
    public class AppointmentDto
    {
        public DateTime AppointmentDate { get; set; } = DateTime.MinValue;
        public string AppointmentNote { get; set; } = string.Empty;
        public string PatientFullName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
    }
}
