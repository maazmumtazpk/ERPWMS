using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedDate { get; set; }
}
