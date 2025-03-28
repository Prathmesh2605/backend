using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Core.Entities;
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

        var totalAmount = expenses.Sum(e => e.Amount);
        var categorySummaries = expenses
            .GroupBy(e => new { e.CategoryId, e.Category!.Name })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = Guid.Parse(g.Key.CategoryId.ToString()),
                CategoryName = g.Key.Name,
                TotalAmount = g.Sum(e => e.Amount),
                Count = expenses.Count(e => e.CategoryId == Guid.Parse(g.Key.CategoryId.ToString())),
                Percentage = (g.Sum(e => e.Amount) / totalAmount) * 100
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        var monthlyTotals = expenses
            .Where(e => e.Date.Date >= request.StartDate.Value.Date && e.Date.Date <= request.EndDate.Value.Date)
            .GroupBy(e => new { e.Date.Month, e.Date.Year })
            .Select(g => new MonthlyTotal
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                Amount = g.Sum(e => e.Amount)
            }).ToList();

        var summary = new ExpenseSummaryDto
        {
            TotalAmount = totalAmount,
            TotalCount = expenses.Count,
            AverageAmount = totalAmount / expenses.Count,
            MaxAmount = expenses.Max(e => e.Amount),
            MinAmount = expenses.Min(e => e.Amount),
            MonthlyTotals = monthlyTotals,
            CategorySummaries = categorySummaries
        };

        return Result<ExpenseSummaryDto>.Success(summary);
    }
}
