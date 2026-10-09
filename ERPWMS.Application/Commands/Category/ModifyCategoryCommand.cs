using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Category
{
    public class ModifyCategoryCommand : IRequest<bool>
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? CategoryCode { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public int UpdatedBy { get; set; }
    }
}