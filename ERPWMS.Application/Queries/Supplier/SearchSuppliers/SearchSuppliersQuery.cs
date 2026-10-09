using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Supplier.SearchSuppliers
{
    public class SearchSuppliersQuery : IRequest<SearchSuppliersResult>
    {
        public string? SearchTerm { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierCode { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? NTN { get; set; }
        public string? TaxId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeDeleted { get; set; } = false;
    }

    public class SearchSuppliersResult
    {
        public List<SupplierListDto> Suppliers { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
