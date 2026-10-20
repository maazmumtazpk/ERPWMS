using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public int BranchId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string? WarehouseCode { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }




    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<CycleCount> CycleCounts { get; set; } = new List<CycleCount>();

    public virtual ICollection<DispatchMaster> DispatchMasters { get; set; } = new List<DispatchMaster>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Grn> Grns { get; set; } = new List<Grn>();

    public virtual ICollection<InventoryValuation> InventoryValuations { get; set; } = new List<InventoryValuation>();

    public virtual ICollection<PickingList> PickingLists { get; set; } = new List<PickingList>();

    public virtual ICollection<PurchaseMaster> PurchaseMasters { get; set; } = new List<PurchaseMaster>();

    public virtual ICollection<SalesMaster> SalesMasters { get; set; } = new List<SalesMaster>();

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

    public virtual ICollection<StockTransfer> StockTransferFromWarehouses { get; set; } = new List<StockTransfer>();

    public virtual ICollection<StockTransfer> StockTransferToWarehouses { get; set; } = new List<StockTransfer>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? DeletedByNavigation { get; set; }
}

