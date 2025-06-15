using System;
using LoanManagement.Domain;

namespace LoanManagement.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    (string token, DateTime expires) GenerateToken(User user);
}