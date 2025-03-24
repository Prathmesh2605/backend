using ExpenseTracker.Core.Enums;

namespace ExpenseTracker.Application.DTOs;

public record ExpenseDto(
    Guid Id,
    decimal Amount,
    string Description,
    DateTime Date,
    ExpenseType Type,
    string Currency,
    Guid CategoryId,
    string CategoryName,
    string? Notes,
    bool IsRecurring,
    string? RecurrencePattern,
    ReceiptDto? Receipt,
    DateTime CreatedAt);
