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
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string? SortBy = null,
    string? SortDirection = null,
    List<Guid>? CategoryIds = null) : IRequest<Result<PaginatedList<ExpenseDto>>>;

public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, Result<PaginatedList<ExpenseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetExpensesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaginatedList<ExpenseDto>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        // Ensure user is authenticated
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return Result<PaginatedList<ExpenseDto>>.Failure("User not authenticated");
        }

        // Parse the user ID to Guid
        if (!Guid.TryParse(_currentUserService.UserId, out Guid userId))
        {
            return Result<PaginatedList<ExpenseDto>>.Failure("Invalid user ID");
        }

        var query = _unitOfWork.Repository<Expense>().GetAll()
            .Include(e => e.Category)
            .Include(e => e.Receipt)
            .AsQueryable();

        // Filter by current user
        query = query.Where(e => e.UserId == userId);

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

        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            query = query.Where(e => request.CategoryIds.Contains(e.CategoryId));
        }

        if(request.MinAmount != null)
        {
            query = query.Where(x => x.Amount >= request.MinAmount);
        }

        if (request.MaxAmount != null)
        {
            query = query.Where(x => x.Amount <= request.MaxAmount);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            // Default sort direction is ascending
            var isAscending = string.IsNullOrEmpty(request.SortDirection) || 
                              request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

            query = ApplySorting(query, request.SortBy, isAscending);
        }
        else
        {
            // Default sorting by date descending
            query = query.OrderByDescending(e => e.Date);
        }

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

    private IQueryable<Expense> ApplySorting(IQueryable<Expense> query, string sortBy, bool isAscending)
    {
        return sortBy.ToLower() switch
        {
            "date" => isAscending ? query.OrderBy(e => e.Date) : query.OrderByDescending(e => e.Date),
            "amount" => isAscending ? query.OrderBy(e => e.Amount) : query.OrderByDescending(e => e.Amount),
            "description" => isAscending ? query.OrderBy(e => e.Description) : query.OrderByDescending(e => e.Description),
            "category" => isAscending ? query.OrderBy(e => e.Category.Name) : query.OrderByDescending(e => e.Category.Name),
            "createdat" => isAscending ? query.OrderBy(e => e.CreatedAt) : query.OrderByDescending(e => e.CreatedAt),
            _ => query.OrderByDescending(e => e.Date) // Default sort
        };
    }
}
