using ExpenseTracker.Core.Common;
using ExpenseTracker.Core.Enums;

namespace ExpenseTracker.Core.Entities;

public class Category : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public CategoryType Type { get; set; }
    public bool IsDefault { get; set; }
    public Guid UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
