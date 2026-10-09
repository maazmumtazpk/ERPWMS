using MediatR;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database.Models;



namespace ERPWMS.Application.Commands.Supplier.SupplierContact.AddSupplierContact
{
    public class AddSupplierContactCommand : IRequest<SupplierContactDto>
    {
        public int SupplierId { get; set; }
        public string ContactName { get; set; } = null!;
        public string? Phone { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public string? Department { get; set; }
        public bool? IsPrimary { get; set; }
        public int CurrentUserId { get; set; }
    }
}
