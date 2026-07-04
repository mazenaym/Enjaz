using Enjaz.Reviews.Application.Reviews;
using Enjaz.Reviews.Infrastructure.Persistence;
using Enjaz.Reviews.Infrastructure.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enjaz.Reviews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReviewsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        services.AddDbContext<ReviewsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IReviewsRepository, ReviewsRepository>();
        return services;
    }
}
