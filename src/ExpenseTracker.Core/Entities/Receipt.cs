using ExpenseTracker.Core.Common;

namespace ExpenseTracker.Core.Entities;

public class Receipt : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
    public Guid ExpenseId { get; set; }

    // Navigation property
    public virtual Expense Expense { get; set; } = null!;
}
