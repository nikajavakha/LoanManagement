using LoanManagement.Application.Options;
using LoanManagement.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoanManagement.Infrastructure.Services;

public class LoanApplicationQueueService : ILoanApplicationQueueService, IAsyncDisposable
{
    private readonly LoanApplicationQueueOptions _options;
    private readonly ILogger<LoanApplicationQueueService> _logger;
    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;
    private Func<int, Task<bool>> _messageHandler;
    private bool _disposed;
    private bool _initialized;
    
    public LoanApplicationQueueService(
        IOptions<LoanApplicationQueueOptions> options,
        ILogger<LoanApplicationQueueService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    private async Task EnsureInitializedAsync()
    {
        if (!_initialized)
        {
            await InitializeAsync();
            _initialized = true;
        }
    }
    private async Task InitializeAsync()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true);

            await _channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueBindAsync(
                queue: _options.QueueName,
                exchange: _options.ExchangeName,
                routingKey: _options.RoutingKey);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("LoanApplicationQueueService initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize LoanApplicationQueueService");
            throw;
        }
    }

    public async Task PublishLoanApplicationAsync(int loanApplicationId)
    {
        await EnsureInitializedAsync();
        
        var body = BitConverter.GetBytes(loanApplicationId);
        var messageId = Guid.NewGuid().ToString();

        var properties = new BasicProperties
        {
            Persistent = true,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            MessageId = messageId
        };

        await _channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Published loan application {LoanId} to queue", loanApplicationId);
    }

    public async Task StartConsumingAsync(Func<int, Task<bool>> messageHandler)
    {
        await EnsureInitializedAsync();
        
        _messageHandler = messageHandler;
        
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(_options.QueueName, false, _consumer);

        _logger.LogInformation("Started consuming messages from queue: {QueueName}", _options.QueueName);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        var deliveryTag = ea.DeliveryTag;
        try
        {
            var loanApplicationId = BitConverter.ToInt32(ea.Body.Span);
            
            _logger.LogInformation("Received message for LoanId: {LoanApplicationId}", loanApplicationId);

            var success = await _messageHandler(loanApplicationId);

            if (success)
            {
                await _channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false);
                _logger.LogDebug("Message acknowledged for LoanId: {LoanApplicationId}", loanApplicationId);
            }
            else
            {
                await _channel.BasicNackAsync(deliveryTag: deliveryTag, multiple: false, requeue: true);
                _logger.LogWarning("Failed to process loan application {LoanId}, requeued", loanApplicationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            await _channel.BasicNackAsync(deliveryTag: deliveryTag, multiple: false, requeue: true);
        }
    }

    public async Task StopConsumingAsync()
    {
        if (_consumer != null && _channel?.IsOpen == true)
        {
            try
            {
                await _channel.BasicCancelAsync(_consumer.ConsumerTags.FirstOrDefault() ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping consumer");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            await StopConsumingAsync();
            
            _consumer = null;
            
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
            
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
            
            _disposed = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disposal");
        }
    }
}