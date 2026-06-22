using CompanyERP.DTOs;

namespace CompanyERP.IServices
{
    public interface IEmployeeProjectService
    {
        Task<AssignEmployeeDto> AssignEmployeeToProjectAsync(AssignEmployeeDto assignEmployeeDto);
        Task<bool> RemoveEmployeeFromProjectAsync(int employeeId,int projectId);

        Task<EmployeeProjectsReportDto> GetEmployeeProjectsReportAsync(int employeeId);

        Task<ProjectEmployeesReportDto> GetProjectEmployeesReportAsync(int projctId);




    }
}
