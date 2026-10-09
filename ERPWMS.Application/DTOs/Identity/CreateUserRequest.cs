using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Identity
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 4)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? EmployeeCode { get; set; }
        public int? WarehouseId { get; set; }
        public bool IsActive { get; set; } = true;

        public int[]? RoleIds { get; set; }
    }
}