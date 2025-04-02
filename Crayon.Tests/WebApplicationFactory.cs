using System.Data.Common;
using Crayon.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject1;

public class CustomWebApplicationFactory<TProgram>(Action<IServiceCollection> servicesOverrides)
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

        //builder.UseEnvironment("Development");
    }
}