using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Enums;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Reports.Queries;

public record GetExpenseSummaryQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? CategoryId = null) : IRequest<Result<ExpenseSummaryDto>>;

public class GetExpenseSummaryQueryHandler : IRequestHandler<GetExpenseSummaryQuery, Result<ExpenseSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetExpenseSummaryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<ExpenseSummaryDto>> Handle(GetExpenseSummaryQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Expense>()
            .GetAll()
            .Include(e => e.Category)
            .Where(e => e.UserId == Guid.Parse(_currentUser.UserId.ToString()));

        if (request.StartDate.HasValue)
        {
            query = query.Where(e => e.Date >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(e => e.Date <= request.EndDate.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == Guid.Parse(request.CategoryId.ToString()));
        }

        var expenses = await query.ToListAsync(cancellationToken);

        if (!expenses.Any())
        {
            return Result<ExpenseSummaryDto>.Success(new ExpenseSummaryDto());
        }

        var expenseTransactions = expenses.Where(x => x.Type == ExpenseType.Expense).ToList();
        var incomeTransactions = expenses.Where(x => x.Type == ExpenseType.Income).ToList();

        // Calculate expense totals with null check to avoid divide by zero
        var totalExpenseAmount = expenseTransactions.Sum(e => e.Amount);
        var averageExpenseAmount = expenseTransactions.Any() ? totalExpenseAmount / expenseTransactions.Count : 0;

        // Calculate income totals with null check to avoid divide by zero
        var totalIncomeAmount = incomeTransactions.Sum(e => e.Amount);
        var averageIncomeAmount = incomeTransactions.Any() ? totalIncomeAmount / incomeTransactions.Count : 0;

        // Calculate category summaries for expenses only
        var categorySummaries = expenseTransactions
            .GroupBy(e => new { e.CategoryId, CategoryName = e.Category?.Name ?? "Uncategorized" })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = Guid.Parse(g.Key.CategoryId.ToString()),
                CategoryName = g.Key.CategoryName,
                TotalAmount = g.Sum(e => e.Amount),
                Count = g.Count(),
                Percentage = totalExpenseAmount > 0 ? (g.Sum(e => e.Amount) / totalExpenseAmount) * 100 : 0
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        // Calculate monthly totals for expenses only
        var monthlyTotals = expenseTransactions
            .GroupBy(e => new { e.Date.Month, e.Date.Year })
            .Select(g => new MonthlyTotal
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                Amount = g.Sum(e => e.Amount)
            }).ToList();

        var summary = new ExpenseSummaryDto
        {
            TotalExpenseAmount = totalExpenseAmount,
            TotalIncomeAmount = totalIncomeAmount,
            TotalCount = expenses.Count,
            AverageExpenseAmount = averageExpenseAmount,
            AverageIncomeAmount = averageIncomeAmount,
            MaxAmount = expenses.Any() ? expenses.Max(e => e.Amount) : 0,
            MinAmount = expenses.Any() ? expenses.Min(e => e.Amount) : 0,
            MonthlyTotals = monthlyTotals,
            CategorySummaries = categorySummaries
        };

        return Result<ExpenseSummaryDto>.Success(summary);
    }
}
