using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Enums;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public record CreateExpenseCommand(
    decimal Amount,
    string Description,
    DateTime Date,
    ExpenseType Type,
    string Currency,
    Guid CategoryId,
    string? Notes,
    bool IsRecurring,
    string? RecurrencePattern,
    IFormFile? Receipt) : IRequest<Result<ExpenseDto>>;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Result<ExpenseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;

    public CreateExpenseCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task<Result<ExpenseDto>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // Validate category exists
        var category = await _unitOfWork.Repository<Category>()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId);

        if (category == null)
        {
            return Result<ExpenseDto>.Failure($"Category with ID {request.CategoryId} not found.");
        }

        // Create expense
        var expense = new Expense
        {
            Amount = request.Amount,
            Description = request.Description,
            Date = request.Date,
            Type = request.Type,
            Currency = request.Currency,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            IsRecurring = request.IsRecurring,
            RecurrencePattern = request.RecurrencePattern
        };

        // Handle receipt upload if present
        if (request.Receipt != null)
        {
            using var stream = request.Receipt.OpenReadStream();
            var fileName = await _fileStorage.SaveFileAsync(
                stream,
                request.Receipt.FileName,
                request.Receipt.ContentType);

            expense.Receipt = new Receipt
            {
                FileName = fileName,
                ContentType = request.Receipt.ContentType,
                FileSize = request.Receipt.Length,
                UploadDate = DateTime.UtcNow
            };
        }

        // Save to database
        await _unitOfWork.Repository<Expense>().AddAsync(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return
        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return Result<ExpenseDto>.Success(expenseDto);
    }
}
