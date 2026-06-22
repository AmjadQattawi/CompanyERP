namespace CompanyERP.DTOs
{
    public class EmployeeProjectsReportDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<AssignedProjectDetailsDto> AssignedProjects { get; set; } = new();
    }

    public class AssignedProjectDetailsDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int HoursAssigned { get; set; }
    }

}
