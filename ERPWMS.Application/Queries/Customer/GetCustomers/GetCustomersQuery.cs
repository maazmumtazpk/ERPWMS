using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Customer.GetCustomers
{
    public class GetCustomersQuery : IRequest<GetCustomersResult>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeDeleted { get; set; } = false;
    }

    public class GetCustomersResult
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
