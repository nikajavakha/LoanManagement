using System.Reflection;
using FluentValidation;
using LoanManagement.Application.Behaviors;
using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Validators.Loan;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LoanManagement.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        
        services.AddAutoMapper(executingAssembly);
        
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(executingAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining<ApproveLoanApplicationCommandValidator>();
        
        return services;
    }
}