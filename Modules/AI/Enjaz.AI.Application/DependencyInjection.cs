using Enjaz.AI.Application.AI;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.AI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAiApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddScoped<IIssueClassifier, IssueClassifier>();

        return services;
    }
}
