namespace ExpenseTracker.Application.DTOs.Reports;

public class MonthlyExpenseDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public List<CategorySummaryDto> CategorySummaries { get; set; } = new();
}
