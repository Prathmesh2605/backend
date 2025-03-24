using ExpenseTracker.Core.Common;

namespace ExpenseTracker.Core.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string PreferredCurrency { get; set; } = "USD";
    public bool EmailNotificationsEnabled { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
