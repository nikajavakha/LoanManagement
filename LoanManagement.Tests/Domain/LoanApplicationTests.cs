using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Tests.Domain;

public class LoanApplicationTests
{
    [Fact]
    public void Submit_WhenStatusIsInProcess_UpdatesStatusAndTimestamp()
    {
        // Arrange
        var loanApplication = CreateLoanApplication();
        loanApplication.Status = LoanStatus.InProcess;
        var beforeSubmit = DateTime.UtcNow;

        // Act
        loanApplication.Submit();

        // Assert
        Assert.Equal(LoanStatus.Submitted, loanApplication.Status);
        Assert.NotNull(loanApplication.SubmittedAt);
        Assert.True(loanApplication.SubmittedAt >= beforeSubmit);
    }

    [Fact]
    public void Submit_WhenStatusIsNotInProcess_ThrowsInvalidOperationException()
    {
        // Arrange
        var loanApplication = CreateLoanApplication();
        loanApplication.Status = LoanStatus.Approved;

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => loanApplication.Submit());
        Assert.Equal("Loan application cannot be submitted in current status", exception.Message);
    }

    [Fact]
    public void Approve_WhenStatusIsPending_UpdatesStatusAndApprover()
    {
        // Arrange
        var loanApplication = CreateLoanApplication();
        loanApplication.Status = LoanStatus.Pending;
        var approverId = 2;

        // Act
        loanApplication.Approve(approverId);

        // Assert
        Assert.Equal(LoanStatus.Approved, loanApplication.Status);
        Assert.Equal(approverId, loanApplication.ApproverId);
        Assert.NotNull(loanApplication.ApprovedAt);
        Assert.Null(loanApplication.RejectionReason);
    }

    [Fact]
    public void Reject_WithEmptyReason_ThrowsArgumentException()
    {
        // Arrange
        var loanApplication = CreateLoanApplication();
        loanApplication.Status = LoanStatus.Pending;

        // Assert
        Assert.Throws<ArgumentException>(() => loanApplication.Reject(2, ""));
        Assert.Throws<ArgumentException>(() => loanApplication.Reject(2, null));
        Assert.Throws<ArgumentException>(() => loanApplication.Reject(2, "   "));
    }

    [Theory]
    [InlineData(LoanStatus.InProcess, true)]
    [InlineData(LoanStatus.Submitted, false)]
    [InlineData(LoanStatus.Pending, false)]
    [InlineData(LoanStatus.Approved, false)]
    [InlineData(LoanStatus.Rejected, false)]
    public void CanBeEdited_ReturnsCorrectValue_BasedOnStatus(LoanStatus status, bool expected)
    {
        // Arrange
        var loanApplication = CreateLoanApplication();
        loanApplication.Status = status;

        // Act
        var result = loanApplication.CanBeEdited();

        // Assert
        Assert.Equal(expected, result);
    }

    private static LoanApplication CreateLoanApplication()
    {
        return new LoanApplication
        {
            Id = 1,
            UserId = 1,
            LoanType = LoanType.FastLoan,
            Amount = 10000m,
            Currency = Currency.GEL,
            PeriodInMonths = 12,
            Status = LoanStatus.InProcess,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}