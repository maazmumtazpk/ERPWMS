using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SalesOrderStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public bool? IsActive { get; set; }
}
