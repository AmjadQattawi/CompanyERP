using System.ComponentModel.DataAnnotations;

namespace CompanyERP.DTOs
{
    public class AssignEmployeeDto
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Hours assigned is required.")]
        [Range(1, 160, ErrorMessage = "Hours assigned must be between 1 and 160 hours.")]
        public int HoursAssigned { get; set; }

    }
}