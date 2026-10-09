using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Supplier.SupplierContact.SetPrimarySupplierContact
{
    public class SetPrimarySupplierContactCommand : IRequest<bool>
    {
        public int ContactId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
