using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Web.ViewModels.Warehouse
{
    public class BranchSelectItem
    {
        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
    }

    public class EditWarehouseViewModel
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Warehouse Name is required")]
        [StringLength(100)]
        [Display(Name = "Warehouse Name")]
        public string WarehouseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Warehouse Code is required")]
        [StringLength(20)]
        [Display(Name = "Warehouse Code")]
        public string WarehouseCode { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Branch Name")]
        public string? BranchName { get; set; }

        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public List<BranchSelectItem> Branches { get; set; } = new();
    }

    public class RegisterZoneViewModel
    {
        public int? ZoneId { get; set; }
        [Required(ErrorMessage = "Zone Name is required")]
        [StringLength(100)]
        [Display(Name = "Zone Name")]
        public string ZoneName { get; set; } = string.Empty;
        [StringLength(20)]
        [Display(Name = "Zone Code")]
        public string? ZoneCode { get; set; }
        [Required(ErrorMessage = "Warehouse is required")]
        [Display(Name = "Warehouse")]
        public int? WarehouseId { get; set; }
        [Display(Name = "Warehouse Name")]
        public string? WarehouseName { get; set; }
        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; } = true;
        public List<object> Warehouses { get; set; } = new();
    }

    public class RegisterRackViewModel
    {
        public int? RackId { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Rack Name")]
        public string RackName { get; set; } = string.Empty;
        [StringLength(20)]
        [Display(Name = "Rack Code")]
        public string? RackCode { get; set; }
        [Required]
        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }
        [Display(Name = "Warehouse")]
        public int? WarehouseId { get; set; }
        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; } = true;

        public List<ZoneSelectItem> Zones { get; set; } = new();
        public List<object> Warehouses { get; set; } = new();
    }

    public class ZoneSelectItem
    {
        public int ZoneId { get; set; }
        public string? ZoneName { get; set; }
        public string? ZoneCode { get; set; }
        public int? WarehouseId { get; set; }
    }

    public class RegisterBinViewModel
    {
        public int? BinId { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Bin Name")]
        public string BinName { get; set; } = string.Empty;
        [StringLength(20)]
        [Display(Name = "Bin Code")]
        public string? BinCode { get; set; }
        [Required]
        [Display(Name = "Rack")]
        public int? RackId { get; set; }
        [Display(Name = "Zone")]
        public int? ZoneId { get; set; }
        [Display(Name = "Warehouse")]
        public int? WarehouseId { get; set; }
        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; } = true;

        public List<ZoneSelectItem> Zones { get; set; } = new();
        public List<object> Racks { get; set; } = new();
        public List<object> Warehouses { get; set; } = new();
    }
}
