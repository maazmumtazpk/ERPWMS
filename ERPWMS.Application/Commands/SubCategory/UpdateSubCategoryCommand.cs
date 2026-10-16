using MediatR;

namespace ERPWMS.Application.Commands.SubCategory
{
    public class UpdateSubCategoryCommand : IRequest<bool>
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryName { get; set; } = null!;
        public string? SubCategoryCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}

