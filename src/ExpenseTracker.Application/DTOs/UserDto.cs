namespace ExpenseTracker.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string? ProfilePicture,
    string PreferredCurrency,
    bool EmailNotificationsEnabled);
