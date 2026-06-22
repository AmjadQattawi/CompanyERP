    using CompanyERP.Enums;

    namespace CompanyERP.Entities
    {
        public class Employee
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PasswordHash {  get; set; } = string.Empty;
            public decimal Salary { get; set; }
            public UserRole Role { get; set; } = UserRole.Employee;
            public int BranchId {  get; set; }
            public Branch Branch { get; set; } = null!;


            public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();


        }
    }