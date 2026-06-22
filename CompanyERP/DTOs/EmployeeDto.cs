using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CompanyERP.Enums;

namespace CompanyERP.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Range(0, 99999999, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }

        [JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public UserRole Role { get; set; }


        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchId { get; set; }

        public string? BranchName { get; set; }
    }
}