using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Expenses.Queries;

public record GetExpensesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? CategoryId = null) : IRequest<Result<PaginatedList<ExpenseDto>>>;

public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, Result<PaginatedList<ExpenseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetExpensesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ExpenseDto>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Expense>().GetAll()
            .Include(e => e.Category)
            .Include(e => e.Receipt)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(e => 
                e.Description.Contains(request.SearchTerm) || 
                e.Notes.Contains(request.SearchTerm));
        }

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
            query = query.Where(e => e.CategoryId == request.CategoryId.Value);
        }

        // Order by date descending
        query = query.OrderByDescending(e => e.Date);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var expenses = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var expenseDtos = _mapper.Map<List<ExpenseDto>>(expenses);

        // Create paginated result
        var paginatedList = new PaginatedList<ExpenseDto>(
            expenseDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PaginatedList<ExpenseDto>>.Success(paginatedList);
    }
}
