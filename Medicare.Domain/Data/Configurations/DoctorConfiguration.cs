using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicare.Domain.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(doctor => doctor.Id);

            builder.HasOne(doctor => doctor.DoctorType)
                   .WithMany(doctorType => doctorType.Doctors)
                   .HasForeignKey(doctor => doctor.DoctorTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(doctor => doctor.Appointments)
                   .WithOne(appointment => appointment.Doctor)
                   .HasForeignKey(appointment => appointment.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(doctor => doctor.Recipes)
                   .WithOne(recipe => recipe.Doctor)
                   .HasForeignKey(recipe => recipe.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
