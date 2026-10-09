using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Supplier.SupplierContact.ActivateSupplierContact
{
    public class ActivateSupplierContactCommandHandler : IRequestHandler<ActivateSupplierContactCommand, bool>
    {
        private readonly AppDbContext _context;

        public ActivateSupplierContactCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ActivateSupplierContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.SupplierContacts
                .Include(sc => sc.Supplier)
                .FirstOrDefaultAsync(sc => sc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new SupplierContactNotFoundException(request.ContactId);
            }

            if (contact.Supplier == null || contact.Supplier.IsDeleted == true)
            {
                throw new SupplierNotFoundException(contact.SupplierId);
            }

            if (contact.Supplier.IsActive != true)
            {
                throw new SupplierInactiveException(contact.SupplierId);
            }

            // Already active
            if (contact.IsActive == true)
            {
                return true;
            }

            contact.IsActive = true;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
