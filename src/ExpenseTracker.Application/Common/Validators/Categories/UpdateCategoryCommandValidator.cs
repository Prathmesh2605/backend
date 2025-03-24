using ExpenseTracker.Application.Features.Categories.Commands;
using FluentValidation;

namespace ExpenseTracker.Application.Common.Validators.Categories;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9 ]+$")
            .WithMessage("Category name can only contain letters, numbers, and spaces");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Icon)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.Icon));

        RuleFor(x => x.Color)
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex color code (e.g., #FF0000)")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid category type");
    }
}
