using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicare.Domain.Data.Configurations
{
    public class RecipeConfigutation 
        : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.HasKey(recipe => recipe.Id);

            builder.Property(recipe => recipe.DurationDays)
                   .HasConversion(
                       d => d.TotalDays,
                       days => TimeSpan.FromDays(days)
                   );

            builder.HasOne(recipe => recipe.Patient)
                   .WithMany(patient => patient.Recipes)
                   .HasForeignKey(recipe => recipe.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(recipe => recipe.Doctor)
                   .WithMany(doctor => doctor.Recipes)
                   .HasForeignKey(recipe => recipe.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
