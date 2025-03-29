using ExpenseTracker.Core.Common;
using ExpenseTracker.Core.Enums;

namespace ExpenseTracker.Core.Entities;

public class Expense : AuditableEntity
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public ExpenseType Type { get; set; }
    public string Currency { get; set; } = "INR";
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public string? Notes { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Receipt? Receipt { get; set; }
}
