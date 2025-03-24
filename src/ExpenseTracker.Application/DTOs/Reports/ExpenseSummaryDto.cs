namespace ExpenseTracker.Application.DTOs.Reports;

public class ExpenseSummaryDto
{
    public decimal TotalAmount { get; set; }
    public int TotalCount { get; set; }
    public decimal AverageAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal MinAmount { get; set; }
    public List<CategorySummaryDto> CategorySummaries { get; set; } = new();
}
