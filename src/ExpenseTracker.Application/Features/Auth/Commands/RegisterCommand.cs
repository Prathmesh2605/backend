using AutoMapper;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IAuthService authService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Repository<User>()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure(
                existingUser.Email == request.Email
                    ? "Email is already registered"
                    : "Username is already taken");
        }

        // Create registration request
        var registerRequest = new RegisterRequest(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        // Register user through auth service
        return await _authService.RegisterAsync(registerRequest);
    }
}
