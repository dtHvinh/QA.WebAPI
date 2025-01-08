using WebAPI.Auto;
using WebAPI.Utilities.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging();

builder.Services.WithConfiguration(builder.Configuration)
                .ConfigureDatabase()
                .ConfigureAuthentication()
                .ConfigureAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.RegisterEndpoints();

app.Run();
