using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Tests.Helpers;

public class TestDataBuilder
{
    public static LoanApplication CreateLoanApplication(
        int id = 1,
        int userId = 1,
        LoanStatus status = LoanStatus.InProcess)
    {
        return new LoanApplication
        {
            Id = id,
            UserId = userId,
            LoanType = LoanType.FastLoan,
            Amount = 10000m,
            Currency = Currency.GEL,
            PeriodInMonths = 12,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}