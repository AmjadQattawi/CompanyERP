using CompanyERP.DTOs;
using CompanyERP.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ProjectDto>> CreateProject(ProjectDto projectDto)
        {
            var createdProject = await _projectService.CreateProjectAsync(projectDto);
            return Ok(createdProject);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            return Ok(project);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(ProjectDto projectDto)
        {
            var updatedProject = await _projectService.UpdateProjectAsync(projectDto);
            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent(); 
        }


        [HttpGet("{id}/budget-report")]
        [Authorize(Roles = "Admin,Manager")] 
        public async Task<ActionResult<ProjectBudgetReportDto>> GetBudgetReport(int id)
        {
            var report = await _projectService.GetProjectBudgetReportAsync(id);

            return Ok(report);
        }

    }
}