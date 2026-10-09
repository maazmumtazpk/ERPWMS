using MediatR;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Supplier.SupplierContact.GetSupplierContactDetails
{
    public class GetSupplierContactDetailsQuery : IRequest<SupplierContactDto>
    {
        public int ContactId { get; set; }
    }
}
