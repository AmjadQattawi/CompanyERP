using CompanyERP.Enums;

namespace CompanyERP.DTOs
{
    public class ProjectEmployeesReportDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public List<AssignedEmployeeDetailsDto> AssignedEmployees { get; set; } = new();
    }

    public class AssignedEmployeeDetailsDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int HoursAssigned { get; set; }


    }

}
