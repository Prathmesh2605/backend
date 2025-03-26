using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Features.Expenses.Commands;
using ExpenseTracker.Application.Features.Expenses.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[Authorize]
public class ExpensesController : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PaginatedList<ExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpenses([FromBody] GetExpensesQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpense(Guid id)
    {
        var query = new GetExpenseByIdQuery(id);
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetExpense), new { id = result.Data!.Id }, result.Data)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var command = new DeleteExpenseCommand(id);
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : NotFound(result.Error);
    }
}
