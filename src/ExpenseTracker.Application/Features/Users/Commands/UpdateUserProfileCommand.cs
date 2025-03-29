using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ExpenseTracker.Application.Features.Users.Commands;

public record UpdateUserProfileCommand(
    string? FirstName,
    string? LastName,
    string? PreferredCurrency
) : IRequest<Result<UserProfileDto>>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return Result<UserProfileDto>.Failure("User is not authenticated");
        }

        var user = await _unitOfWork.Repository<User>()
            .GetAll()
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken);

        if (user == null)
        {
            return Result<UserProfileDto>.Failure("User not found.");
        }

        // Update basic info
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.PreferredCurrency = request.PreferredCurrency ?? user.PreferredCurrency;
        //user.TimeZone = request.TimeZone ?? user.TimeZone;


        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userProfileDto = _mapper.Map<UserProfileDto>(user);
        return Result<UserProfileDto>.Success(userProfileDto);
    }
}
