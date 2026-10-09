using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Identity
{
    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}