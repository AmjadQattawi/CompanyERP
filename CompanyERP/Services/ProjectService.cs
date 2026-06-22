using CompanyERP.Data;
using CompanyERP.DTOs;
using CompanyERP.Entities;
using CompanyERP.IServices;
using CompanyERP.Exceptions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CompanyERP.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        public readonly IMapper _mapper;

        public ProjectService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projectsFromDb = await _context.Project.ToListAsync();
            var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projectsFromDb);
            return projectsDto;
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
        {
            var newProject = _mapper.Map<Project>(projectDto);
            
            _context.Project.Add(newProject);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProjectDto>(newProject);
        }



        public async Task<ProjectDto> GetProjectByIdAsync(int id)
        {
            var project = await _context.Project.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                throw new NotFoundException($"Project with ID {id} was not found.");
            }
            return _mapper.Map<ProjectDto>(project);
           
        }

        public async Task<ProjectDto> UpdateProjectAsync(ProjectDto projectDto)
        {
            var existingProject = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectDto.Id);
            if (existingProject == null)
            {
                throw new NotFoundException($"Cannot update. Project with ID {projectDto.Id} was not found.");
            }
            _mapper.Map(projectDto, existingProject);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProjectDto>(existingProject);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Project.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                throw new NotFoundException($"Cannot delete. Project with ID {id} was not found.");
            }
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<ProjectBudgetReportDto> GetProjectBudgetReportAsync(int projectId)
        {
            var project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                throw new NotFoundException($"Project with ID {projectId} was not found.");
            }
            var employeeProjects = await _context.EmployeeProject
                .Include(ep => ep.Employee) 
                .Where(ep => ep.ProjectId == projectId)
                .ToListAsync();

            decimal totalActualCost = 0;

            foreach (var ep in employeeProjects)
            {
                if (ep.Employee != null)
                {
                    decimal regularHourlyRate = ep.Employee.Salary / 160;

                    decimal employeeProjectCost = ep.HoursAssigned * regularHourlyRate;

                    totalActualCost += employeeProjectCost;
                }
            }

            decimal remainingBudget = project.Budget - totalActualCost;

            string budgetStatus = "On Track"; 
            if (remainingBudget < 0)
            {
                budgetStatus = "Exceeded"; 
            }
            else if (remainingBudget < (project.Budget * 0.2m))
            {
                budgetStatus = "Under Risk";
            }

            return new ProjectBudgetReportDto
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                Budget = project.Budget,
                ActualCost = totalActualCost,
                RemainingBudget = remainingBudget,
                Status = budgetStatus
            };
        }



    }
}