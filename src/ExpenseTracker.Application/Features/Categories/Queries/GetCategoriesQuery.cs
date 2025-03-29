using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Enums;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Categories.Queries;

public record GetCategoriesQuery(CategoryType? Type = null) : IRequest<Result<List<CategoryDto>>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        List<CategoryDto> categoryDtos= new List<CategoryDto>();
        try
        {
            var query = _unitOfWork.Repository<Category>().GetAll();

            if (request.Type.HasValue)
            {
                query = query.Where(c => c.Type == request.Type.Value);
            }

            var categories = await query
                .Where(c => c.UserId == Guid.Parse(_currentUserService.UserId.ToString()))
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Result<List<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return Result<List<CategoryDto>>.Failure(ex.Message);
        }

    }
}
