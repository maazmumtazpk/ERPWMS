using MediatR;
using ERPWMS.Application.DTOs.Supplier;

namespace ERPWMS.Application.Commands.Supplier.ChangeSupplierProfile
{
    public class ChangeSupplierProfileCommand : IRequest<SupplierDto>
    {
        public int SupplierId { get; set; }
        public RegisterSupplierRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
