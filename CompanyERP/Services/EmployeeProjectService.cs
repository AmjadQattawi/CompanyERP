using AutoMapper;
using CompanyERP.Data;
using CompanyERP.DTOs;
using CompanyERP.Entities;
using CompanyERP.Exceptions;
using CompanyERP.IServices;
using Microsoft.EntityFrameworkCore;

namespace CompanyERP.Services
{
    public class EmployeeProjectService : IEmployeeProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper; 

        public EmployeeProjectService(AppDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AssignEmployeeDto> AssignEmployeeToProjectAsync(AssignEmployeeDto assignEmployeeDto)
        {
            bool employeeExist = await _context.Employee.AnyAsync(e => e.Id == assignEmployeeDto.EmployeeId);

            if (!employeeExist)
            {
                throw new NotFoundException($"Employee with ID {assignEmployeeDto.EmployeeId} was not found.");
            }

            bool projectExist = await _context.Project.AnyAsync(p => p.Id == assignEmployeeDto.ProjectId);
            if (!projectExist)
            {
                throw new NotFoundException($"Project with ID {assignEmployeeDto.ProjectId} was not found");
            }

            bool alreadyAssigned = await _context.EmployeeProject
                                        .AnyAsync(ep => ep.EmployeeId == assignEmployeeDto.EmployeeId
                                                     && ep.ProjectId == assignEmployeeDto.ProjectId);
            if (alreadyAssigned)
            {
                throw new InvalidOperationException($"Employee with ID {assignEmployeeDto.EmployeeId} is already assigned to Project ID {assignEmployeeDto.ProjectId}.");
            }

            int currentAssignedHours = await _context.EmployeeProject.
                Where(ep => ep.EmployeeId == assignEmployeeDto.EmployeeId).
                SumAsync(ep => ep.HoursAssigned);
            if (currentAssignedHours + assignEmployeeDto.HoursAssigned > 160)
            {
                throw new MaxWorkHoursExceededException($"The total hours across all projects cannot" +
                    $" exceed 160. The employee currently has {currentAssignedHours} hours assigned.");
            }
            var newAssignment = _mapper.Map<EmployeeProject>(assignEmployeeDto);

            _context.EmployeeProject.Add(newAssignment);
            await _context.SaveChangesAsync();

            return _mapper.Map<AssignEmployeeDto>(newAssignment);
        }

        public async Task<EmployeeProjectsReportDto> GetEmployeeProjectsReportAsync(int employeeId)
        {
            var employee = await _context.Employee
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {employeeId} was not found.");
            }

            return _mapper.Map<EmployeeProjectsReportDto>(employee);
        }

        public async Task<ProjectEmployeesReportDto> GetProjectEmployeesReportAsync(int projectId)
        {
            var project = await _context.Project
                .Include(p => p.EmployeeProjects)
                    .ThenInclude(ep => ep.Employee)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                throw new NotFoundException($"Project with ID {projectId} was not found.");
            }

            return _mapper.Map<ProjectEmployeesReportDto>(project);
        }

        public async Task<bool> RemoveEmployeeFromProjectAsync(int employeeId, int projectId)
        {
            bool employeeExist = await _context.Employee.AnyAsync(e => e.Id == employeeId);
            if (!employeeExist) 
            {
                throw new NotFoundException($"Employee with ID {employeeId} was not found.");
            }
            bool projectExist = await _context.Project.AnyAsync(p => p.Id == projectId);
            if (!projectExist)
            {
                throw new NotFoundException($"Project with ID {projectId} was not found");
            }
                
            var alreadyAssigned = await _context.EmployeeProject.FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId 
            && ep.ProjectId == projectId);
            if (alreadyAssigned == null)
            {
                throw new NotFoundException($"This employee with id {employeeId} wasn't even hired for this" +
                    $" project with id {projectId}, so I can't remove him.");
            }

             _context.EmployeeProject.Remove(alreadyAssigned);
             await _context.SaveChangesAsync();
                
            return true;
        }
    }
}
