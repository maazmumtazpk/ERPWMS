using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Product
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public int DeletedBy { get; set; }
    }
}
