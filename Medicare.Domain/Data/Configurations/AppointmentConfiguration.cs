using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicare.Domain.Data.Configurations
{
    public class AppointmentConfiguration 
        : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(appointment => appointment.Id);

            builder.HasOne(appointment => appointment.Patient)
                   .WithMany(patient => patient.Appointments)
                   .HasForeignKey(appointment => appointment.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(appointment => appointment.Doctor)
                   .WithMany(doctor => doctor.Appointments)
                   .HasForeignKey(appointment => appointment.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
