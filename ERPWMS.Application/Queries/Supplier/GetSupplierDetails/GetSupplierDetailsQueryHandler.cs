using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Supplier.GetSupplierDetails
{
    public class GetSupplierDetailsQueryHandler : IRequestHandler<GetSupplierDetailsQuery, SupplierDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetSupplierDetailsQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierDto> Handle(GetSupplierDetailsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Suppliers
                .Include(s => s.SupplierContacts)
                .Include(s => s.SupplierContacts)
                .AsQueryable();

            if (!request.IncludeDeleted)
            {
                query = query.Where(s => s.IsDeleted != true);
            }

            var supplier = await query
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId, cancellationToken);

            if (supplier == null)
            {
                throw new SupplierNotFoundException(request.SupplierId);
            }

            var result = _mapper.Map<SupplierDto>(supplier);
            return result;
        }
    }
}
