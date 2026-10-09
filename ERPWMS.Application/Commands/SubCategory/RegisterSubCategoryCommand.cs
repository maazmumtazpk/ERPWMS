using MediatR;

namespace ERPWMS.Application.Commands.SubCategory
{
    public class RegisterSubCategoryCommand : IRequest<int>
    {
        public int CategoryId { get; set; }
        public string SubCategoryName { get; set; } = null!;
        public string? SubCategoryCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
