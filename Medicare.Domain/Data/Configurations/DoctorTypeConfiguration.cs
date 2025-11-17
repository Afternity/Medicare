using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicare.Domain.Data.Configurations
{
    public class DoctorTypeConfiguration 
        : IEntityTypeConfiguration<DoctorType>
    {
        public void Configure(EntityTypeBuilder<DoctorType> builder)
        {
            builder.HasKey(doctorType => doctorType.Id);

            builder.HasMany(doctorType => doctorType.Doctors)
                   .WithOne(doctor => doctor.DoctorType)
                   .HasForeignKey(doctor => doctor.DoctorTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
