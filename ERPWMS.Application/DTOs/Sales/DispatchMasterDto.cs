
using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Sales
{
    public class DispatchMasterDto
    {
        public int DispatchId { get; set; }

        public int SalesId { get; set; }

        public string SalesOrderNo { get; set; } = string.Empty;

        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; } = string.Empty;

        public string DispatchNo { get; set; } = string.Empty;

        public DateTime? DispatchDate { get; set; }

        public string CarrierName { get; set; } = string.Empty;

        public string? TrackingNo { get; set; }

        public string? Status { get; set; }

        public int? DispatchedBy { get; set; }

        public string? DispatchedByName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? WorkflowInstanceId { get; set; }

        public int? CurrentStageId { get; set; }

        public string? CurrentStageName { get; set; }

        public string CreatedBy { get; set; }

        public string? CreatedByName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String?UpdatedBy{ get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public List<DispatchDetailDto> DispatchDetails { get; set; } = new();
    }
}

