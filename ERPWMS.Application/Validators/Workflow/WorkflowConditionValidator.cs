using ERPWMS.Application.DTOs.Workflow;
using FluentValidation;

namespace ERPWMS.Application.Validators.Workflow
{
    public class CreateWorkflowConditionValidator : AbstractValidator<CreateWorkflowConditionRequest>
    {
        public CreateWorkflowConditionValidator()
        {
            RuleFor(x => x.WorkflowDefinitionId)
                .NotEmpty().WithMessage("Valid Workflow Definition is required.");

            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("Field name is required.")
                .MaximumLength(100).WithMessage("Field name cannot exceed 100 characters.");

            RuleFor(x => x.Operator)
                .NotEmpty().WithMessage("Operator is required.")
                .MaximumLength(20).WithMessage("Operator cannot exceed 20 characters.")
                .Must(op => new[] { "==", "!=", ">", "<", ">=", "<=", "Contains", "StartsWith", "EndsWith" }.Contains(op))
                .WithMessage("Invalid operator.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Value is required.")
                .MaximumLength(255).WithMessage("Value cannot exceed 255 characters.");
        }
    }

    public class UpdateWorkflowConditionValidator : AbstractValidator<UpdateWorkflowConditionRequest>
    {
        public UpdateWorkflowConditionValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Invalid condition ID.");

            RuleFor(x => x.WorkflowDefinitionId)
                .NotEmpty().WithMessage("Valid Workflow Definition is required.");

            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("Field name is required.")
                .MaximumLength(100).WithMessage("Field name cannot exceed 100 characters.");

            RuleFor(x => x.Operator)
                .NotEmpty().WithMessage("Operator is required.")
                .MaximumLength(20).WithMessage("Operator cannot exceed 20 characters.")
                .Must(op => new[] { "==", "!=", ">", "<", ">=", "<=", "Contains", "StartsWith", "EndsWith" }.Contains(op))
                .WithMessage("Invalid operator.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Value is required.")
                .MaximumLength(255).WithMessage("Value cannot exceed 255 characters.");
        }
    }
}