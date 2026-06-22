using System.ComponentModel.DataAnnotations;

namespace CompanyERP.DTOs
{
    public class BranchDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        public string BranchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Branch code is required.")]
        [StringLength(10, ErrorMessage = "Branch code cannot exceed 10 characters.")]
        public string BranchCode { get; set; } = string.Empty;
    }
}
