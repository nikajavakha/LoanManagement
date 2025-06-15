using LoanManagement.Domain.Enums;
using LoanManagement.Tests.Helpers;

namespace LoanManagement.Tests.Integration;

public class LoanApplicationWorkflowTests
{
    [Fact]
    public void CompleteWorkflow_FromCreationToApproval_WorksCorrectly()
    {
        // Arrange
        var loanApplication = TestDataBuilder.CreateLoanApplication();
        var approverId = 2;

        // Step 1: Initial state
        Assert.Equal(LoanStatus.InProcess, loanApplication.Status);
        Assert.True(loanApplication.CanBeEdited());
        Assert.True(loanApplication.CanBeSubmitted());

        // Step 2: Submit
        loanApplication.Submit();
        Assert.Equal(LoanStatus.Submitted, loanApplication.Status);
        Assert.False(loanApplication.CanBeEdited());
        Assert.NotNull(loanApplication.SubmittedAt);

        // Step 3: Move to pending 
        loanApplication.MoveToPending();
        Assert.Equal(LoanStatus.Pending, loanApplication.Status);
        Assert.True(loanApplication.CanBeApproved());
        Assert.True(loanApplication.CanBeRejected());

        // Step 4: Approve
        loanApplication.Approve(approverId);
        Assert.Equal(LoanStatus.Approved, loanApplication.Status);
        Assert.Equal(approverId, loanApplication.ApproverId);
        Assert.False(loanApplication.CanBeApproved());
        Assert.NotNull(loanApplication.ApprovedAt);
    }

    [Fact]
    public void RejectWorkflow_WithReason_WorksCorrectly()
    {
        var loanApplication = TestDataBuilder.CreateLoanApplication();
        var approverId = 2;
        var rejectionReason = "Insufficient income verification";

        // Move to pending state
        loanApplication.Submit();
        loanApplication.MoveToPending();

        //  Reject
        loanApplication.Reject(approverId, rejectionReason);
        Assert.Equal(LoanStatus.Rejected, loanApplication.Status);
        Assert.Equal(approverId, loanApplication.ApproverId);
        Assert.Equal(rejectionReason, loanApplication.RejectionReason);
        Assert.NotNull(loanApplication.RejectedAt);
        Assert.False(loanApplication.CanBeRejected());
    }

    [Fact]
    public void InvalidStateTransitions_ThrowExceptions()
    {
        // Arrange
        var loanApplication = TestDataBuilder.CreateLoanApplication();
        
        // Cannot approve without being pending
        Assert.Throws<InvalidOperationException>(() => loanApplication.Approve(2));
        
        // Cannot reject without being pending
        Assert.Throws<InvalidOperationException>(() => loanApplication.Reject(2, "reason"));
        
        // Submit and try to edit
        loanApplication.Submit();
        Assert.Throws<InvalidOperationException>(() => 
            loanApplication.Update(LoanType.AutoLoan, 20000m, Currency.USD, 24));
    }
}