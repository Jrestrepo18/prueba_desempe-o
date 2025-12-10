using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Employee Configuration =====
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Document)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.Property(e => e.Position)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(20);
                
                entity.Property(e => e.EducationLevel)
                    .HasMaxLength(50);
                
                entity.Property(e => e.DepartmentId)
                    .HasColumnType("integer");
                
                // Foreign key relationship with Department
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.Property(e => e.Salary)
                    .HasPrecision(18, 2);
                
                // DateTime value converters to ensure UTC
                entity.Property(e => e.BirthDate)
                    .HasConversion(
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
                    );
                
                entity.Property(e => e.HireDate)
                    .HasConversion(
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
                    );
                
                entity.Property(e => e.CreatedAt)
                    .HasConversion(
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    );
                
                entity.Property(e => e.UpdatedAt)
                    .HasConversion(
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
                    );
            });

            // ===== Department Configuration =====
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);
                
                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(d => d.Description)
                    .HasMaxLength(300);
            });
        }
    }
}
