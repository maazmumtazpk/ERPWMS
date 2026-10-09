using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Customer.SearchCustomers
{
    public class SearchCustomersQuery : IRequest<SearchCustomersResult>
    {
        public string? SearchTerm { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerCode { get; set; }
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

    public class SearchCustomersResult
    {
        public List<CustomerListDto> Customers { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
