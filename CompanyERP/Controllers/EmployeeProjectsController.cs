using CompanyERP.Data;
using CompanyERP.DTOs;
using CompanyERP.IServices;
using CompanyERP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeProjectsController : ControllerBase
    {
        private readonly IEmployeeProjectService _employeeProjectService;
        public EmployeeProjectsController(IEmployeeProjectService employeeProjectService)
        {
            _employeeProjectService = employeeProjectService;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AssignEmployeeDto>> AssignEmployeeToProject(AssignEmployeeDto assignEmployeeDto)
        {
            var assignEP = await _employeeProjectService.AssignEmployeeToProjectAsync(assignEmployeeDto);
            return Ok(assignEP);
        }


        [HttpDelete("remove")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> RemoveEmployeeFromProject([FromQuery] int employeeId, [FromQuery] int projectId)
        {
            await _employeeProjectService.RemoveEmployeeFromProjectAsync(employeeId, projectId);
            return NoContent();
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<EmployeeProjectsReportDto>> GetEmployeeProjectsReport(int employeeId)
        {
            var empReport = await _employeeProjectService.GetEmployeeProjectsReportAsync(employeeId);

            return Ok(empReport);
        }


        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<ProjectEmployeesReportDto>> GetProjectEmployeesReport(int projectId)
        {
            var projectReport = await _employeeProjectService.GetProjectEmployeesReportAsync(projectId);
            return Ok(projectReport);
        }




    }
}
