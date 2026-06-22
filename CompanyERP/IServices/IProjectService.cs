using CompanyERP.DTOs;

namespace CompanyERP.IServices
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto);

        Task<ProjectDto> GetProjectByIdAsync(int id);
        Task<ProjectDto> UpdateProjectAsync(ProjectDto projectDto);
        Task<bool> DeleteProjectAsync(int id);

        Task<ProjectBudgetReportDto> GetProjectBudgetReportAsync(int projectId);



    }
}