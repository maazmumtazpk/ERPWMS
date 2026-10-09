using MediatR;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Customer.GetCustomerDetails
{
    public class GetCustomerDetailsQuery : IRequest<CustomerDto>
    {
        public int CustomerId { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
