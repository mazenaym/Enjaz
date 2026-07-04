using Enjaz.Reviews.Application.Reviews;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Reviews.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddReviewsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IReviewsService, ReviewsService>();
        services.AddScoped<IReviewSentimentAnalyzer, FakeReviewSentimentAnalyzer>();
        return services;
    }
}
