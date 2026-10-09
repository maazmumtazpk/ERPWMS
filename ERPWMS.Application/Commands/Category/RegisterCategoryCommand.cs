using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Category
{
    public class RegisterCategoryCommand : IRequest<int>
    {
        public string CategoryName { get; set; } = null!;
        public string? CategoryCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
