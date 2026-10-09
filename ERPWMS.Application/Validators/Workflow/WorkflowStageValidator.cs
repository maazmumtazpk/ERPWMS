using ERPWMS.Application.DTOs.Workflow;
using FluentValidation;

namespace ERPWMS.Application.Validators.Workflow
{
    public class CreateWorkflowStageValidator : AbstractValidator<CreateWorkflowStageRequest>
    {
        public CreateWorkflowStageValidator()
        {
            RuleFor(x => x.WorkflowDefinitionId)
                .NotEmpty().WithMessage("Valid Workflow Definition is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Stage name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Stage code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Code can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order must be zero or greater.");
        }
    }

    public class UpdateWorkflowStageValidator : AbstractValidator<UpdateWorkflowStageRequest>
    {
        public UpdateWorkflowStageValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Invalid stage ID.");

            RuleFor(x => x.WorkflowDefinitionId)
                .NotEmpty().WithMessage("Valid Workflow Definition is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Stage name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Stage code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Code can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order must be zero or greater.");
        }
    }
}