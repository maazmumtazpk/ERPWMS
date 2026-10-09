using System;
using ERPWMS.Database.Models;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;


namespace ERPWMS.Application.Commands.Supplier.ActivateSupplier
{
    public class ActivateSupplierCommandHandler : IRequestHandler<ActivateSupplierCommand, bool>
    {
        private readonly AppDbContext _context;

        public ActivateSupplierCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ActivateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId && s.IsDeleted != true, cancellationToken);

            if (supplier == null)
            {
                throw new SupplierNotFoundException(request.SupplierId);
            }

            // Already active
            if (supplier.IsActive == true)
            {
                return true;
            }

            supplier.IsActive = true;
            supplier.UpdatedBy = request.CurrentUserId.ToString();
            supplier.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
