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
    string? Currency,
    string? TimeZone,
    IFormFile? Avatar,
    string? ContentType) : IRequest<Result<UserProfileDto>>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorage,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorage = fileStorage;
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
        user.PreferredCurrency = request.Currency ?? user.PreferredCurrency;
        //user.TimeZone = request.TimeZone ?? user.TimeZone;

        // Handle avatar upload
        if (request.Avatar != null)
        {
            var fileStream = request.Avatar.OpenReadStream();
            var fileName = Path.GetFileName(request.Avatar.FileName);
            var avatarPath = await _fileStorage.SaveFileAsync(
                fileStream,
                fileName,
                request.ContentType ?? request.Avatar.ContentType
            );

            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                await _fileStorage.DeleteFileAsync(user.ProfilePicture);
            }

            user.ProfilePicture = avatarPath;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userProfileDto = _mapper.Map<UserProfileDto>(user);
        return Result<UserProfileDto>.Success(userProfileDto);
    }
}
