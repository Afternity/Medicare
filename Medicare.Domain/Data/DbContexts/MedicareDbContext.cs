using Medicare.Domain.Data.Configurations;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medicare.Domain.Data.DbContexts
{
    public class MedicareDbContext 
        : DbContext
    {
        public MedicareDbContext(
         DbContextOptions<MedicareDbContext> options)
         : base(options)
        {
        }

        public MedicareDbContext()
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<DoctorType> DoctorTypes { get; set; }
        public virtual DbSet<MedicalCard> MedicalCards { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=AFTERNITY;Initial Catalog=MedicareDB;Integrated Security=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalCardConfiguration());
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new RecipeConfigutation());
        }
    }
}
