using MediatR;
using ERPWMS.Application.DTOs.Supplier;

namespace ERPWMS.Application.Commands.Supplier.RegisterSupplier
{
    public class RegisterSupplierCommand : IRequest<SupplierDto>
    {
        public RegisterSupplierRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
