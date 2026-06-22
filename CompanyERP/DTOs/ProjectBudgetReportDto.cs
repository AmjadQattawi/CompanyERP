namespace CompanyERP.DTOs
{
    public class ProjectBudgetReportDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal Budget { get; set; } 
        public decimal ActualCost { get; set; } 
        public decimal RemainingBudget { get; set; } 
        public string Status { get; set; } 
    }
}