using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Identity
{
    public class UpdateRoleRequest
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}