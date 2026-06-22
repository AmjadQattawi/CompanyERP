using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyERP.Entities
{
    public class Payroll
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public int Month { get; set; } 

        [Required]
        public int Year { get; set; } 

        [Required]
        public decimal BasicSalary { get; set; } 

        public decimal OvertimeMoney { get; set; }

        public decimal DeductionMoney { get; set; }

        [Required]
        public decimal NetSalary { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}