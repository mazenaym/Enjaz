using Enjaz.Identity.Application.Auth;
using Microsoft.Extensions.Hosting;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class AppEnvironment(IHostEnvironment hostEnvironment) : IAppEnvironment
{
    public bool IsDevelopment => hostEnvironment.IsDevelopment();
}
