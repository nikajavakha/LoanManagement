using LoanManagement.Application.Commands.Auth;
using LoanManagement.Application.DTOs.Auth;
using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Interfaces.Services;
using LoanManagement.Domain;
using LoanManagement.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RegisterCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.PersonalNumber == request.PersonalNumber || 
                                      u.PersonalNumber == request.PersonalNumber, cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this username or personal number already exists");
        }

        var user = new User
        {
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PersonalNumber = request.PersonalNumber,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Role = Roles.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _tokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}