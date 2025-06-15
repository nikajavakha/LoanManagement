using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace LoanManagement.Infrastructure.Services;

public class LoanApplicationQueueConsumerService : BackgroundService
{
    private readonly ILoanApplicationQueueService _queueService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoanApplicationQueueConsumerService> _logger;

    public LoanApplicationQueueConsumerService(
        ILoanApplicationQueueService queueService,
        IServiceProvider serviceProvider,
        ILogger<LoanApplicationQueueConsumerService> logger)
    {
        _queueService = queueService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LoanApplicationQueueConsumerService started");

        try
        {
            await _queueService.StartConsumingAsync(ProcessLoanApplicationMessage);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoanApplicationQueueConsumerService");
            throw;
        }
    }

    private async Task<bool> ProcessLoanApplicationMessage(int loanApplicationId)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new MoveToPendingApplicationCommand(loanApplicationId);
            var result = await mediator.Send(command);

            _logger.LogInformation("Processed loan application: {LoanId}", loanApplicationId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process loan application: {LoanId}", loanApplicationId);
            return false;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LoanApplicationQueueConsumerService stopping");
        await _queueService.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}