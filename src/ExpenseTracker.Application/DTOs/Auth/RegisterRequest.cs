namespace ExpenseTracker.Application.DTOs.Auth;

public record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName);
