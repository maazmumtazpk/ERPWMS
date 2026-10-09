using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class UpdatePickingListRequest
    {
        public int PickingId { get; set; }

        [Required(ErrorMessage = "Sales Order is required.")]
        public int SalesId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Picking Number is required.")]
        [StringLength(50)]
        public string PickingNo { get; set; } = string.Empty;

        public DateTime? PickingDate { get; set; }

        public int? AssignedTo { get; set; }

        public string? Status { get; set; }

        public List<CreatePickingDetailRequest> PickingDetails { get; set; } = new();
    }
}