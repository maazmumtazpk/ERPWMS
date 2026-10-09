namespace ERPWMS.Application.DTOs.EmployeeBankAccount
{
    public class CreateEmployeeBankAccountRequest
    {
        public int EmployeeId { get; set; }

        public string BankName { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;

        public string? Iban { get; set; }

        public string? AccountType { get; set; }

        public bool? IsPrimary { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}