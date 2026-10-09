using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Identity
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }

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

        public string? EmployeeCode { get; set; }
        public int? WarehouseId { get; set; }
        public bool? IsActive { get; set; }

        public int[]? RoleIds { get; set; }
    }
}