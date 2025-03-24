using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Categories.Commands;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Get category
        var category = await _unitOfWork.Repository<Category>()
            .GetAll()
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            return Result.Failure($"Category with ID {request.Id} not found.");
        }

        // Check if category has expenses
        if (category.Expenses.Any())
        {
            return Result.Failure("Cannot delete category that has expenses. Please delete or reassign the expenses first.");
        }

        // Check if category is default
        if (category.IsDefault)
        {
            return Result.Failure("Cannot delete default category.");
        }

        // Delete category
        await _unitOfWork.Repository<Category>().DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
