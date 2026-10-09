using System;
using ERPWMS.Database.Models;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;


namespace ERPWMS.Application.Commands.Supplier.DeactivateSupplier
{
    public class DeactivateSupplierCommandHandler : IRequestHandler<DeactivateSupplierCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeactivateSupplierCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId && s.IsDeleted != true, cancellationToken);

            if (supplier == null)
            {
                throw new SupplierNotFoundException(request.SupplierId);
            }

            // Already inactive
            if (supplier.IsActive != true)
            {
                return true;
            }

            supplier.IsActive = false;
            supplier.UpdatedBy = request.CurrentUserId.ToString();
            supplier.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
