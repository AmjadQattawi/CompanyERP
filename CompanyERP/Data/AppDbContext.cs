using CompanyERP.Entities;
using CompanyERP.Enums;
using Microsoft.EntityFrameworkCore;

namespace CompanyERP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Branch> Branch { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<EmployeeProject> EmployeeProject { get; set; }

        public DbSet<Payroll> Payroll { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Payroll>().Property(p => p.BasicSalary).HasPrecision(18, 2);
            modelBuilder.Entity<Payroll>().Property(p => p.OvertimeMoney).HasPrecision(18, 2);
            modelBuilder.Entity<Payroll>().Property(p => p.DeductionMoney).HasPrecision(18, 2);
            modelBuilder.Entity<Payroll>().Property(p => p.NetSalary).HasPrecision(18, 2);

            // Composite Key for EmployeeProject
            modelBuilder.Entity<EmployeeProject>()
                .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasPrecision(18, 2);
            modelBuilder.Entity<Project>().Property(p => p.Budget).HasPrecision(18, 2); // 💡 ضبطنا هاد السطر عشان يروح الـ Warning

            modelBuilder.Entity<Branch>().HasData(new Branch
            {
                Id = 99,
                BranchName = "Main Headquarter",
                BranchCode = "HQ01"
            });

            var adminUser = new Employee
            {
                Id = 99,
                FullName = "System Admin",
                Email = "admin@company.com",
                Salary = 2500,
                Role = UserRole.Admin,
                BranchId = 99
            };

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Employee>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

            modelBuilder.Entity<Employee>().HasData(adminUser);

            base.OnModelCreating(modelBuilder);
        }
    }
}