using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Enums;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Icon,
    string? Color,
    CategoryType Type,
    bool IsDefault) : IRequest<Result<CategoryDto>>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Get category
        var category = await _unitOfWork.Repository<Category>()
            .GetAll()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            return Result<CategoryDto>.Failure($"Category with ID {request.Id} not found.");
        }

        // Check if another category with same name exists
        var existingCategory = await _unitOfWork.Repository<Category>()
            .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id != request.Id);

        if (existingCategory != null)
        {
            return Result<CategoryDto>.Failure($"Category with name '{request.Name}' already exists.");
        }

        // Update category
        category.Name = request.Name;
        category.Description = request.Description;
        category.Icon = request.Icon;
        category.Color = request.Color;
        category.Type = request.Type;
        category.IsDefault = request.IsDefault;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Result<CategoryDto>.Success(categoryDto);
    }
}
