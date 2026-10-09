using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Web.ViewModels.Branch
{
    public class RegisterBranchViewModel
    {
        public int? BranchId { get; set; }

        [Required(ErrorMessage = "Branch Name is required")]
        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters")]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Branch Code is required")]
        [StringLength(20, ErrorMessage = "Branch Code cannot exceed 20 characters")]
        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; } = true;
    }
}
