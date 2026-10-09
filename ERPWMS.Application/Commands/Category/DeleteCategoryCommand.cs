using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Category
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public int CategoryId { get; set; }
        public int DeletedBy { get; set; }
    }
}
