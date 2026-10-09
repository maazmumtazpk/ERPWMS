using MediatR;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Supplier.GetSupplierDetails
{
    public class GetSupplierDetailsQuery : IRequest<SupplierDto>
    {
        public int SupplierId { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
