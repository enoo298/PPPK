using Microsoft.EntityFrameworkCore;

namespace PPPK_Enver_Besic.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Definicija DbSet-ova za svaki entitet
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExaminationImage> ExaminationImages { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            // 1. Patient - MedicalRecord (1:N)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.MedicalRecords)
                .WithOne(mr => mr.Patient)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Patient - Examination (1:N)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Examinations)
                .WithOne(e => e.Patient)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Examination - ExaminationImage (1:N)
            modelBuilder.Entity<Examination>()
                .HasMany(e => e.ExaminationImages)
                .WithOne(ei => ei.Examination)
                .HasForeignKey(ei => ei.ExaminationId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Patient - Prescription (1:N)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Prescriptions)
                .WithOne(pr => pr.Patient)
                .HasForeignKey(pr => pr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dodatne konfiguracije (npr. indeksi, jedinstvenost OIB-a, itd.)
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.OIB)
                .IsUnique();
        }
    }
}
