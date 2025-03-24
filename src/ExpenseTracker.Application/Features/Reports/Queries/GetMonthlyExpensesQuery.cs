using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Reports.Queries;

public record GetMonthlyExpensesQuery(int Year, int? Month = null) : IRequest<Result<List<MonthlyExpenseDto>>>;

public class GetMonthlyExpensesQueryHandler : IRequestHandler<GetMonthlyExpensesQuery, Result<List<MonthlyExpenseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetMonthlyExpensesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<List<MonthlyExpenseDto>>> Handle(GetMonthlyExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Expense>()
            .GetAll()
            .Include(e => e.Category)
            .Where(e => e.UserId == Guid.Parse(_currentUser.UserId.ToString()) && e.Date.Year == request.Year);

        if (request.Month.HasValue)
        {
            query = query.Where(e => e.Date.Month == request.Month.Value);
        }

        var expenses = await query.ToListAsync(cancellationToken);

        var monthlyExpenses = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g =>
            {
                var totalAmount = g.Sum(e => e.Amount);
                var categorySummaries = g
                    .GroupBy(e => new { e.CategoryId, e.Category!.Name })
                    .Select(cg => new CategorySummaryDto
                    {
                        CategoryId = cg.Key.CategoryId,
                        CategoryName = cg.Key.Name,
                        TotalAmount = cg.Sum(e => e.Amount),
                        Count = cg.Count(),
                        Percentage = (cg.Sum(e => e.Amount) / totalAmount) * 100
                    })
                    .OrderByDescending(c => c.TotalAmount)
                    .ToList();

                return new MonthlyExpenseDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = totalAmount,
                    Count = g.Count(),
                    CategorySummaries = categorySummaries
                };
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        return Result<List<MonthlyExpenseDto>>.Success(monthlyExpenses);
    }
}
