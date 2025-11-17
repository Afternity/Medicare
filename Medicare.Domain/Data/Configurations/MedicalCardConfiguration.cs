using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicare.Domain.Data.Configurations
{
    public class MedicalCardConfiguration 
        : IEntityTypeConfiguration<MedicalCard>
    {
        public void Configure(EntityTypeBuilder<MedicalCard> builder)
        {
            builder.HasKey(medicalCard => medicalCard.Id);

            builder.HasOne(medicalCard => medicalCard.Patient)
                   .WithMany(patient => patient.MedicalCards)
                   .HasForeignKey(medicalCard => medicalCard.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
