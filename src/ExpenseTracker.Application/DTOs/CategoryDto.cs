using ExpenseTracker.Core.Enums;

namespace ExpenseTracker.Application.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    string? Icon,
    string? Color,
    CategoryType Type,
    bool IsDefault,
    DateTime CreatedAt);
