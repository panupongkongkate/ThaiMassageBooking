using Microsoft.EntityFrameworkCore;
using MassageBookingApi.Models;

namespace MassageBookingApi.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Masseur> Masseurs { get; set; }
        public DbSet<SystemLineApiUsage> SystemLineApiUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.CustomerName).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.MasseurName).HasMaxLength(50);
                entity.Property(e => e.TimeSlot).HasMaxLength(20);
                entity.Property(e => e.Pincode).HasMaxLength(10);
                
                // Create index for efficient querying
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.Date, e.MasseurName, e.TimeSlot });
            });

            modelBuilder.Entity<Masseur>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                
                // Create index for name search
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
            });

            // Seed initial masseur data
            modelBuilder.Entity<SystemLineApiUsage>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Create index for LastResetDate
                entity.HasIndex(e => e.LastResetDate);
            });

            // Seed initial system usage data
            modelBuilder.Entity<SystemLineApiUsage>().HasData(
                new SystemLineApiUsage { Id = 1, TotalMessagesSent = 0, MaxMessagesLimit = 50, LastResetDate = new DateTime(2024, 1, 1), CreatedAt = new DateTime(2024, 1, 1), UpdatedAt = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<Masseur>().HasData(
                new Masseur { Id = 1, Name = "นางสาวสมใจ", Description = "ช่างนวดมืออาชีพ 5 ปี", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Masseur { Id = 2, Name = "นางสาวมาลี", Description = "ผู้เชี่ยวชาญนวดแผนไทย", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Masseur { Id = 3, Name = "นางจันทร์", Description = "นวดผ่อนคลายกล้ามเนื้อ", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Masseur { Id = 4, Name = "นายสมชาย", Description = "นวดกดจุด สปอร์ต", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}