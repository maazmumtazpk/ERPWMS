using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeeStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }
}
