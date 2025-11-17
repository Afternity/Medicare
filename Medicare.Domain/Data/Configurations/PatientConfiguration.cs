using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicare.Domain.Data.Configurations
{
    public class PatientConfiguration 
        : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(patient => patient.Id);

            builder.HasMany(patient => patient.MedicalCards)
                   .WithOne(medicalCard => medicalCard.Patient)
                   .HasForeignKey(medicalCard => medicalCard.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(patient => patient.Appointments)
                   .WithOne(appointment => appointment.Patient)
                   .HasForeignKey(appointment => appointment.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(patient => patient.Recipes)
                   .WithOne(recipe => recipe.Patient)
                   .HasForeignKey(recipe => recipe.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
