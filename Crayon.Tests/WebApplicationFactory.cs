using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Crayon.Tests;

public class CustomWebApplicationFactory<TProgram>(Action<IServiceCollection> servicesOverrides,TestJWTLibrary.Generator generator)
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            if (servicesOverrides != null)
                servicesOverrides(services);
        });
        generator.ConfigureAuthentication(builder);
        builder.UseEnvironment("Test");
    }
}