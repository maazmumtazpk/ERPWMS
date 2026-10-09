using ERPWMS.Application.DTOs.Workflow;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Validators.Workflow
{
    public class CreateWorkflowDefinitionValidator : AbstractValidator<CreateWorkflowDefinitionRequest>
    {
        public CreateWorkflowDefinitionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Code can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.EntityType)
                .MaximumLength(50).WithMessage("Entity type cannot exceed 50 characters.");
        }
    }

    public class UpdateWorkflowDefinitionValidator : AbstractValidator<UpdateWorkflowDefinitionRequest>
    {
        public UpdateWorkflowDefinitionValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Invalid definition ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Code can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.EntityType)
                .MaximumLength(50).WithMessage("Entity type cannot exceed 50 characters.");
        }
    }
}