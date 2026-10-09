using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Brand
{
    public class DeleteBrandCommand : IRequest<bool>
    {
        public int BrandId { get; set; }
        public int DeletedBy { get; set; }
    }
}
