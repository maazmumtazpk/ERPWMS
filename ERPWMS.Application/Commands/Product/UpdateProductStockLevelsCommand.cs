using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Product
{
    public class UpdateProductStockLevelsCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public int? MinStockLevel { get; set; }
        public int? MaxStockLevel { get; set; }
        public int? ReorderLevel { get; set; }
        public String?UpdatedBy{ get; set; }
    }
}
