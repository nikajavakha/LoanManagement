using LoanManagement.Application.Commands.Auth;
using LoanManagement.Application.DTOs.Auth;
using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PersonalNumber == request.PersonalNumber, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var (token, expiresAt) = _tokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}