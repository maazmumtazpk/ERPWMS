using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class UpdateDispatchRequest
    {
        public int DispatchId { get; set; }

        [Required(ErrorMessage = "Sales Order is required.")]
        public int SalesId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Dispatch Number is required.")]
        [StringLength(50)]
        public string DispatchNo { get; set; } = string.Empty;

        public DateTime? DispatchDate { get; set; }

        [Required(ErrorMessage = "Carrier Name is required.")]
        [StringLength(100)]
        public string CarrierName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TrackingNo { get; set; }

        public string? Remarks { get; set; }

        public string? Status { get; set; }

        public List<CreateDispatchDetailRequest> DispatchDetails { get; set; } = new();
    }
}