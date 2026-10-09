using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Supplier.SupplierContact.GetSupplierContacts
{
    public class GetSupplierContactsQuery : IRequest<GetSupplierContactsResult>
    {
        public int SupplierId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetSupplierContactsResult
    {
        public List<SupplierContactDto> Contacts { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
