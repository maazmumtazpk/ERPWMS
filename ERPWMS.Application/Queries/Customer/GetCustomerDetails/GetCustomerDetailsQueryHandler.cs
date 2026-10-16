using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Customer.GetCustomerDetails
{
    public class GetCustomerDetailsQueryHandler : IRequestHandler<GetCustomerDetailsQuery, CustomerDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetCustomerDetailsQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerDto> Handle(GetCustomerDetailsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Customers
                .Include(c => c.CustomerContacts)
                .Include(c => c.CustomerContacts)
                .AsQueryable();

            if (!request.IncludeDeleted)
            {
                query = query.Where(c => c.IsDeleted != true);
            }

            var customer = await query
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

            if (customer == null)
            {
                throw new CustomerNotFoundException(request.CustomerId);
            }

            var result = _mapper.Map<CustomerDto>(customer);
            return result;
        }
    }
}

