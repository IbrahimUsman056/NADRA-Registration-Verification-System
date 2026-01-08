using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NADRASystem.Models;

namespace NADRASystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CitizenUpdateRequest> CitizenUpdateRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique CNIC
            modelBuilder.Entity<Citizen>()
                .HasIndex(c => c.CNIC)
                .IsUnique();

            // Configure CitizenUpdateRequest relationships
            modelBuilder.Entity<CitizenUpdateRequest>()
                .HasOne(cur => cur.Citizen)
                .WithMany(c => c.UpdateRequests)
                .HasForeignKey(cur => cur.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitizenUpdateRequest>()
                .HasOne(cur => cur.Department)
                .WithMany(d => d.UpdateRequests)
                .HasForeignKey(cur => cur.RequestedByDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // DepartmentId can be null

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "NADRA Admin", DepartmentType = "Government", IsActive = true },
                new Department { DepartmentId = 2, DepartmentName = "Union Council", DepartmentType = "Government", IsActive = true },
                new Department { DepartmentId = 3, DepartmentName = "Bank", DepartmentType = "Financial", IsActive = true },
                new Department { DepartmentId = 4, DepartmentName = "Police", DepartmentType = "Law Enforcement", IsActive = true }
            );

            // Seed default admin user (we'll handle this in Program.cs)
            // Note: For Identity users, we'll create them programmatically
        }
    }
}