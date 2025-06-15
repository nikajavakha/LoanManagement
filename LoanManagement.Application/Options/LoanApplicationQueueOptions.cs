namespace LoanManagement.Application.Options;

public class LoanApplicationQueueOptions
{
    public const string SectionName = "LoanApplicationQueue";
    
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public string RoutingKey { get; set; } 
    public string VirtualHost { get; set; }
}