using FluentValidation;
using JobBoard.Models.DTOs.Jobs;

namespace JobBoard.Validators
{
    public class CreateJobValidator : AbstractValidator<CreateJobDto>
    {
        public CreateJobValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(50).WithMessage("Description must be at least 50 characters");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required");

            RuleFor(x => x.SalaryRange)
                .NotEmpty().WithMessage("Salary range is required");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Please select a category");
        }
    }
}
