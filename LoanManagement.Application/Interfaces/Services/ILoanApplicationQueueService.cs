namespace LoanManagement.Application.Services;

public interface ILoanApplicationQueueService
{
    Task PublishLoanApplicationAsync(int loanApplicationId);
    Task StartConsumingAsync(Func<int, Task<bool>> messageHandler);
    Task StopConsumingAsync();
}