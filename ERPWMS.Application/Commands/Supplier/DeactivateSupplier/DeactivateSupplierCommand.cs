using MediatR;

namespace ERPWMS.Application.Commands.Supplier.DeactivateSupplier
{
    public class DeactivateSupplierCommand : IRequest<bool>
    {
        public int SupplierId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
