using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Supplier.SupplierContact.GetSupplierContacts
{
    public class GetSupplierContactsQueryHandler : IRequestHandler<GetSupplierContactsQuery, GetSupplierContactsResult>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetSupplierContactsQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetSupplierContactsResult> Handle(GetSupplierContactsQuery request, CancellationToken cancellationToken)
        {
            // Validate supplier exists
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId && s.IsDeleted != true, cancellationToken);

            if (supplier == null)
            {
                throw new SupplierNotFoundException(request.SupplierId);
            }

            var query = _context.SupplierContacts
                .Where(sc => sc.SupplierId == request.SupplierId)
                .AsQueryable();

            if (!request.IncludeInactive)
            {
                query = query.Where(sc => sc.IsActive == true);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var contacts = await query
                .OrderByDescending(sc => sc.IsPrimary)
                .ThenBy(sc => sc.ContactName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Set SupplierName for each contact
            foreach (var contact in contacts)
            {
                contact.Supplier = supplier;
            }

            var result = new GetSupplierContactsResult
            {
                Contacts = _mapper.Map<List<SupplierContactDto>>(contacts),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}
