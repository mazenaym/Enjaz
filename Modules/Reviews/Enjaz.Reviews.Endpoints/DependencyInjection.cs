using Enjaz.Reviews.Application;
using Enjaz.Reviews.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Reviews.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddReviewsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReviewsApplication();
        services.AddReviewsInfrastructure(configuration);
        return services;
    }
}
