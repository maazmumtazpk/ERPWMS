using MediatR;

namespace ERPWMS.Application.Commands.Supplier.ActivateSupplier
{
    public class ActivateSupplierCommand : IRequest<bool>
    {
        public int SupplierId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
