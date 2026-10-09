using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Supplier.SupplierContact.ActivateSupplierContact
{
    public class ActivateSupplierContactCommand : IRequest<bool>
    {
        public int ContactId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
