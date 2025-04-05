using Crayon.Configuration;
using Crayon.Endpoints;
using Crayon.Helpers;
using Crayon.Middleware;
using Crayon.Mock;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.RegisterServices(appSettings);

if (appSettings.ConfigureJWT)
{
    TestJWTLibrary.Generator generator = new();
    generator.ConfigureAuthentication(builder.WebHost);
    var defaultToken = generator.GenerateJwt(additionalClaims:new Claim("customer_id","8debd754-286d-4944-8fb5-1a48440f3848"));
    Console.WriteLine(defaultToken);
}

var app = builder.Build();

await app.StartSubscribers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.RegisterEndpoints();

MockServer.Start(appSettings.WiremockPort);
app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.RunAsync();

public partial class Program{}
