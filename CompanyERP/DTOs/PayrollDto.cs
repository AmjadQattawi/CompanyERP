namespace CompanyERP.DTOs
{
    public class PayrollDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } 
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal OvertimeMoney { get; set; }
        public decimal DeductionMoney { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}