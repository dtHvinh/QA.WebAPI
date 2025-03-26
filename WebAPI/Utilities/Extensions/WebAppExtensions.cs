using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

namespace WebAPI.Utilities.Extensions;

public static class WebAppExtensions
{
    public static WebApplication ConfigureFullTextSearch(this WebApplication app)
    {
        var serviceScope = app.Services.CreateAsyncScope();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.ExecuteSqlAsync($"CREATE FULLTEXT CATALOG qa_catalog AS DEFAULT; ");

        return app;
    }
}
