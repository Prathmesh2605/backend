using ExpenseTracker.Application.DTOs.Reports;
using ExpenseTracker.Application.Features.Reports.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[Authorize]
public class ReportsController : ApiControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ExpenseSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpenseSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? categoryId = null)
    {
        var query = new GetExpenseSummaryQuery(startDate, endDate, categoryId);
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("monthly/{year:int}")]
    [ProducesResponseType(typeof(List<MonthlyExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthlyExpenses(int year, [FromQuery] int? month = null)
    {
        var query = new GetMonthlyExpensesQuery(year, month);
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Error);
    }
}
