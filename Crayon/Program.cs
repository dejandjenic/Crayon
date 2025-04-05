using Crayon.Configuration;
using Crayon.Endpoints;
using Crayon.Helpers;
using Crayon.Middleware;
using Crayon.Mock;
using System.Security.Claims;
using Crayon;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.RegisterServices(appSettings);

if (appSettings.ConfigureJWT)
{
    TestJWTLibrary.Generator generator = new();
    generator.ConfigureAuthentication(builder.WebHost);
    var defaultToken = generator.GenerateJwt(additionalClaims:new Claim(Constants.CustomerIdClaimName,Constants.FirstCustomerId));
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
