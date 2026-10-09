using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public String?UpdatedBy{ get; set; }
    }
}
