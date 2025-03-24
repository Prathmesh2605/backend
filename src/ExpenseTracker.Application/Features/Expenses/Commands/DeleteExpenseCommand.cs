using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public record DeleteExpenseCommand(Guid Id) : IRequest<Result>;

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;

    public DeleteExpenseCommandHandler(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        // Get expense with receipt
        var expense = await _unitOfWork.Repository<Expense>()
            .GetAll()
            .Include(e => e.Receipt)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null)
        {
            return Result.Failure($"Expense with ID {request.Id} not found.");
        }

        // Delete receipt file if exists
        if (expense.Receipt != null)
        {
            await _fileStorage.DeleteFileAsync(expense.Receipt.FileName);
        }

        // Delete expense from database
        await _unitOfWork.Repository<Expense>().DeleteAsync(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
