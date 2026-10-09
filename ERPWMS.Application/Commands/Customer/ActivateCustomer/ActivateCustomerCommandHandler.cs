using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.ActivateCustomer
{
    public class ActivateCustomerCommandHandler : IRequestHandler<ActivateCustomerCommand, bool>
    {
        private readonly AppDbContext _context;

        public ActivateCustomerCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ActivateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId && c.IsDeleted != true, cancellationToken);

            if (customer == null)
            {
                throw new CustomerNotFoundException(request.CustomerId);
            }

            if (customer.IsActive == true)
            {
                return true;
            }

            customer.IsActive = true;
            customer.UpdatedBy = request.CurrentUserId.ToString();
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
