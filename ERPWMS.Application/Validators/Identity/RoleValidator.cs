using ERPWMS.Application.DTOs.Identity;
using ERPWMS.Domain.Services;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Validators.Identity
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
    {
        private readonly IRoleService _roleService;

        public CreateRoleValidator(IRoleService roleService)
        {
            _roleService = roleService;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(2).WithMessage("Role name must be at least 2 characters.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.")
                .MustAsync(BeUniqueRole).WithMessage("Role name already exists.");
        }

        private async Task<bool> BeUniqueRole(string name, CancellationToken cancellationToken)
        {
            return !await _roleService.RoleExistsAsync(name);
        }
    }

    public class UpdateRoleValidator : AbstractValidator<UpdateRoleRequest>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleValidator(IRoleService roleService)
        {
            _roleService = roleService;

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Invalid role.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(2).WithMessage("Role name must be at least 2 characters.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.")
                .MustAsync(BeUniqueRole).WithMessage("Role name already exists.");
        }

        private async Task<bool> BeUniqueRole(UpdateRoleRequest request, string name, CancellationToken cancellationToken)
        {
            return !await _roleService.RoleExistsAsync(name, request.RoleId);
        }
    }
}