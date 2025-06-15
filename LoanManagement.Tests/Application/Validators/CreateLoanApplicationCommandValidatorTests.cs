using FluentValidation.TestHelper;
using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Validators.Loan;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Tests.Application.Validators;

public class CreateLoanApplicationCommandValidatorTests
{
    private readonly CreateLoanApplicationCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Should_Have_Error_When_Amount_Is_Invalid(decimal amount)
    {
        // Arrange
        var command = CreateValidCommand() with { Amount = amount };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(2000000)] // Over 1M limit
    [InlineData(5000000)]
    public void Should_Have_Error_When_Amount_Exceeds_Maximum(decimal amount)
    {
        // Arrange
        var command = CreateValidCommand() with { Amount = amount };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(500)] // Over 360 month limit
    public void Should_Have_Error_When_PeriodInMonths_Is_Invalid(int period)
    {
        // Arrange
        var command = CreateValidCommand() with { PeriodInMonths = period };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PeriodInMonths);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateLoanApplicationCommand CreateValidCommand()
    {
        return new CreateLoanApplicationCommand(
            UserId: 1,
            LoanType: LoanType.FastLoan,
            Amount: 10000m,
            Currency: Currency.GEL,
            PeriodInMonths: 12
        );
    }
}