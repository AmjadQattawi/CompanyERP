using AutoMapper;
using CompanyERP.Entities;
using CompanyERP.DTOs;

namespace CompanyERP.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Branch, BranchDto>();
            CreateMap<BranchDto, Branch>();

            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName));
            CreateMap<EmployeeDto, Employee>();

            CreateMap<EmployeeProject, AssignedProjectDetailsDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.ProjectName));

            CreateMap<Employee, EmployeeProjectsReportDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.AssignedProjects, opt => opt.MapFrom(src => src.EmployeeProjects));


            CreateMap<EmployeeProject, AssignedEmployeeDetailsDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Employee.Email));

            CreateMap<Project, ProjectEmployeesReportDto>()
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.ProjectName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) 
                .ForMember(dest => dest.AssignedEmployees, opt => opt.MapFrom(src => src.EmployeeProjects));

            CreateMap<AssignEmployeeDto, EmployeeProject>();
            CreateMap<EmployeeProject, AssignEmployeeDto>();

            CreateMap<Payroll, PayrollDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));

        }
    }
}