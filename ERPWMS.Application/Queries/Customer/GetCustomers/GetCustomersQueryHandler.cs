using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Customer.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, GetCustomersResult>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetCustomersQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetCustomersResult> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Customers.AsQueryable();

            if (!request.IncludeDeleted)
            {
                query = query.Where(c => c.IsDeleted != true);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var customers = await query
                .OrderByDescending(c => c.CustomerId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            foreach (var customer in customers)
            {
                customer.CustomerContacts = await _context.CustomerContacts
                    .Where(cc => cc.CustomerId == customer.CustomerId && cc.IsActive == true)
                    .ToListAsync(cancellationToken);
            }

            var result = new GetCustomersResult
            {
                Customers = _mapper.Map<List<CustomerListDto>>(customers),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}

