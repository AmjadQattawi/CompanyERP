    using CompanyERP.Enums;

    namespace CompanyERP.Entities
    {
        public class Project
        {
            public int Id { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public decimal Budget { get; set; }
            public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

            public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();


        }
    }
