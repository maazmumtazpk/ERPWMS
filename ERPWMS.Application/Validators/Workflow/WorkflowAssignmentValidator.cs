using ERPWMS.Application.DTOs.Workflow;
using FluentValidation;

namespace ERPWMS.Application.Validators.Workflow
{
    public class CreateWorkflowAssignmentValidator : AbstractValidator<CreateWorkflowAssignmentRequest>
    {
        public CreateWorkflowAssignmentValidator()
        {
            RuleFor(x => x.WorkflowInstanceId)
                .NotEmpty().WithMessage("Valid Workflow Instance is required.");
            RuleFor(x => x.StageId)
                .NotEmpty().WithMessage("Stage is required.");
            RuleFor(x => x)
                .Must(x => x.UserId.HasValue || x.RoleId.HasValue)
                .WithMessage("Either User or Role must be assigned.");
        }
    }

    // We can create similar for Update if needed.
}