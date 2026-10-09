using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Supplier.GetSuppliers
{
    public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, GetSuppliersResult>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetSuppliersQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetSuppliersResult> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!request.IncludeDeleted)
            {
                query = query.Where(s => s.IsDeleted != true);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var suppliers = await query
                .OrderByDescending(s => s.SupplierId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Load contact count for each supplier
            foreach (var supplier in suppliers)
            {
                supplier.SupplierContacts = await _context.SupplierContacts
                    .Where(sc => sc.SupplierId == supplier.SupplierId && sc.IsActive == true)
                    .ToListAsync(cancellationToken);
            }

            var result = new GetSuppliersResult
            {
                Suppliers = _mapper.Map<List<SupplierListDto>>(suppliers),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}
