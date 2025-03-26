using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Enums;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public record UpdateExpenseCommand(
    Guid Id,
    decimal Amount,
    string Description,
    DateTime Date,
    ExpenseType Type,
    string Currency,
    Guid CategoryId,
    string? Notes,
    bool IsRecurring,
    string? RecurrencePattern,
    string? ReceiptPath) : IRequest<Result<ExpenseDto>>;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Result<ExpenseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;

    public UpdateExpenseCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task<Result<ExpenseDto>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        // Get existing expense
        var expense = await _unitOfWork.Repository<Expense>()
            .GetAll()
            .Include(e => e.Category)
            .Include(e => e.Receipt)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null)
        {
            return Result<ExpenseDto>.Failure($"Expense with ID {request.Id} not found.");
        }

        // Validate category exists
        var category = await _unitOfWork.Repository<Category>()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId);

        if (category == null)
        {
            return Result<ExpenseDto>.Failure($"Category with ID {request.CategoryId} not found.");
        }

        // Update expense properties
        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.Date = request.Date;
        expense.Type = request.Type;
        expense.Currency = request.Currency;
        expense.CategoryId = request.CategoryId;
        expense.Notes = request.Notes;
        expense.IsRecurring = request.IsRecurring;
        expense.RecurrencePattern = request.RecurrencePattern;

        // Handle receipt update if present
        if (request.ReceiptPath != null)
        {
            // Delete old receipt if exists
            if (expense.Receipt != null)
            {
                await _fileStorage.DeleteFileAsync(expense.Receipt.FileName);
            }

            if (expense.Receipt == null)
            {
                expense.Receipt = new Receipt();
            }

            expense.Receipt.FileName = request.ReceiptPath;
            expense.Receipt.UploadDate = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return Result<ExpenseDto>.Success(expenseDto);
    }
}
