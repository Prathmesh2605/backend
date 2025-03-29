namespace ExpenseTracker.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PreferredCurrency { get; set; }
    //public string? TimeZone { get; set; }
    // string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    //public DateTime? LastLoginAt { get; set; }
}
