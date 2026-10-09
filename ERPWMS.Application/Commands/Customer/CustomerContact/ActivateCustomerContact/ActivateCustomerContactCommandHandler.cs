using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Customer.CustomerContact.ActivateCustomerContact
{
    public class ActivateCustomerContactCommandHandler : IRequestHandler<ActivateCustomerContactCommand, bool>
    {
        private readonly AppDbContext _context;

        public ActivateCustomerContactCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ActivateCustomerContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.CustomerContacts
                .Include(cc => cc.Customer)
                .FirstOrDefaultAsync(cc => cc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new CustomerContactNotFoundException(request.ContactId);
            }

            if (contact.Customer == null || contact.Customer.IsDeleted == true)
            {
                throw new CustomerNotFoundException(contact.CustomerId);
            }

            if (contact.Customer.IsActive != true)
            {
                throw new CustomerInactiveException(contact.CustomerId);
            }

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
