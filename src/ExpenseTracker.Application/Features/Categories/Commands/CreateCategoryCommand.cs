using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Enums;
using ExpenseTracker.Core.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    string? Icon,
    string? Color,
    CategoryType Type,
    bool IsDefault = false) : IRequest<Result<CategoryDto>>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Check if category with same name exists
        var existingCategory = await _unitOfWork.Repository<Category>()
            .FirstOrDefaultAsync(c => c.Name == request.Name);

        if (existingCategory != null)
        {
            return Result<CategoryDto>.Failure($"Category with name '{request.Name}' already exists.");
        }

        // Create category
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            Color = request.Color,
            Type = request.Type,
            IsDefault = request.IsDefault
        };

        await _unitOfWork.Repository<Category>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Result<CategoryDto>.Success(categoryDto);
    }
}
