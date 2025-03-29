namespace ExpenseTracker.Application.DTOs.Reports;

public class ExpenseSummaryDto
{
    public decimal TotalExpenseAmount { get; set; }
    public int TotalCount { get; set; }
    public decimal AverageExpenseAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal MinAmount { get; set; }
    public decimal TotalIncomeAmount { get; set; }
    public decimal AverageIncomeAmount { get; set; }
    public List<MonthlyTotal> MonthlyTotals { get; set; }
    public List<CategorySummaryDto> CategorySummaries { get; set; } = new();
}
