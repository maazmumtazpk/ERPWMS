using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Supplier.SupplierContact.DeactivateSupplierContact
{
    public class DeactivateSupplierContactCommand : IRequest<bool>
    {
        public int ContactId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
