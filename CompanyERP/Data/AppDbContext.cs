using CompanyERP.Entities;
using CompanyERP.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace CompanyERP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // دمجنا الـ IHttpContextAccessor بأمان مع الاحتفاظ بالـ Options الأصلية لمشروعك
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Branch> Branch { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<EmployeeProject> EmployeeProject { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; } // إضافة الجدول رسمياً هنا

        // دالة الـ Override السحرية لالتقاط حركات الجداول تلقائياً وتحويلها لـ JSON
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
                              ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name
                              ?? "System/Worker";

            var auditEntries = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditLog = new AuditLog
                {
                    UserId = currentUser,
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow
                };

                var oldValues = new Dictionary<string, object>();
                var newValues = new Dictionary<string, object>();

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            if (property.CurrentValue != null)
                                newValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            if (property.OriginalValue != null)
                                oldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified && !Equals(property.OriginalValue, property.CurrentValue))
                            {
                                oldValues[propertyName] = property.OriginalValue!;
                                newValues[propertyName] = property.CurrentValue!;
                            }
                            break;
                    }
                }

                if (oldValues.Count > 0) auditLog.OldValues = JsonSerializer.Serialize(oldValues);
                if (newValues.Count > 0) auditLog.NewValues = JsonSerializer.Serialize(newValues);

                auditEntries.Add(auditLog);
            }

            if (auditEntries.Count > 0)
            {
                await AuditLog.AddRangeAsync(auditEntries, cancellationToken);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payroll>().Property(p => p.BasicSalary).HasPrecision(18, 2);
            modelBuilder.Entity<Payroll>().Property(p => p.OvertimeMoney).HasPrecision(18, 2);
            modelBuilder.Entity<Payroll>().Property(p => p.DeductionMoney).HasPrecision(18, 2);
            modelBuilder.Entity<Payroll>().Property(p => p.NetSalary).HasPrecision(18, 2);

            modelBuilder.Entity<EmployeeProject>()
                .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasPrecision(18, 2);
            modelBuilder.Entity<Project>().Property(p => p.Budget).HasPrecision(18, 2);

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