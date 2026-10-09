using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Customer.CustomerContact.GetCustomerContacts
{
    public class GetCustomerContactsQuery : IRequest<GetCustomerContactsResult>
    {
        public int CustomerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetCustomerContactsResult
    {
        public List<CustomerContactDto> Contacts { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
