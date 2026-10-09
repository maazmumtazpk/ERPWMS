using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Web.ViewModels.Warehouse
{
    public class RegisterWarehouseViewModel
    {
        public int? WarehouseId { get; set; }

        [Required(ErrorMessage = "Warehouse Name is required")]
        [StringLength(100, ErrorMessage = "Warehouse Name cannot exceed 100 characters")]
        [Display(Name = "Warehouse Name")]
        public string WarehouseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Warehouse Code is required")]
        [StringLength(20, ErrorMessage = "Warehouse Code cannot exceed 20 characters")]
        [Display(Name = "Warehouse Code")]
        public string WarehouseCode { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string? State { get; set; }

        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }

        [StringLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Branch Name")]
        public string? BranchName { get; set; }

        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; } = true;
    }
}
