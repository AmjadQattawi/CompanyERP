using System.ComponentModel.DataAnnotations;
using CompanyERP.Enums;

namespace CompanyERP.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(150, ErrorMessage = "Project name cannot exceed 150 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Budget is required.")]
        [Range(0, 999999999.99, ErrorMessage = "Budget must be a positive number.")]
        public decimal Budget { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;
    }
}