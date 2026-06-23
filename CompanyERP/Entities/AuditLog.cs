using System;
using System.ComponentModel.DataAnnotations;

namespace CompanyERP.Entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = "System/Worker"; 

        [Required]
        public string Action { get; set; } = string.Empty; 

        [Required]
        public string TableName { get; set; } = string.Empty; 

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? OldValues { get; set; } 

        public string? NewValues { get; set; } 
    }
}