using ExpenseTracker.Application.Features.Expenses.Commands;
using FluentValidation;

namespace ExpenseTracker.Application.Common.Validators.Expenses;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be a valid 3-letter ISO currency code (e.g., USD)");

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        //When(x => x.IsRecurring, () =>
        //{
        //    RuleFor(x => x.RecurrencePattern)
        //        .NotEmpty()
        //        .WithMessage("Recurrence pattern is required for recurring expenses");
        //});

        //When(x => x.Receipt != null, () =>
        //{
        //    RuleFor(x => x.Receipt!.Length)
        //        .LessThanOrEqualTo(10 * 1024 * 1024)
        //        .WithMessage("Receipt file size must not exceed 10MB");

        //    RuleFor(x => x.Receipt!.ContentType)
        //        .Must(x => x.StartsWith("image/") || x == "application/pdf")
        //        .WithMessage("Receipt must be an image or PDF file");
        //});
    }
}
