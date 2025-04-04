using Crayon.Configuration;
using Crayon.Endpoints;
using Crayon.Helpers;
using Crayon.Mock;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.RegisterServices(appSettings);

var app = builder.Build();
await app.StartSubscribers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.RegisterEndpoints();

MockServer.Start();

app.Run();

public partial class Program{}
