using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;
using static WebAPI.Utilities.Enums;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IAdminRepository))]
public class AdminRepository(ApplicationDbContext dbContext) : RepositoryBase<ApplicationUser>(dbContext), IAdminRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    private async Task<GrownAnalytic> InternalGetDailyGrownAnalytic<T>() where T : class, IEntityWithTime<int>
    {
        var previousDate = DateTime.UtcNow.AddDays(-1);
        var currentDate = DateTime.UtcNow;

        var perviousCount = await _dbContext.Set<T>().CountAsync(x => x.CreationDate.Date == previousDate.Date);
        var currentCount = await _dbContext.Set<T>().CountAsync(x => x.CreationDate.Date == currentDate.Date);

        return new GrownAnalytic(perviousCount, currentCount, MathHelper.GetPercentageGrowth(perviousCount, currentCount));
    }

    private async Task<GrownAnalytic> InternalGetWeeklyGrownAnalytic<T>() where T : class, IEntityWithTime<int>
    {
        var now = DateTime.UtcNow;

        var currentWeekStart = now.AddDays(-(int)now.DayOfWeek).Date;
        var previousWeekStart = currentWeekStart.AddDays(-7);
        var previousWeekEnd = currentWeekStart.AddTicks(-1);

        var previousCount = await _dbContext.Set<T>()
            .CountAsync(x => x.CreationDate >= previousWeekStart && x.CreationDate <= previousWeekEnd);
        var currentCount = await _dbContext.Set<T>()
            .CountAsync(x => x.CreationDate >= currentWeekStart && x.CreationDate <= now);

        var a = MathHelper.GetPercentageGrowth(previousCount, currentCount);

        return new GrownAnalytic(previousCount, currentCount, a);
    }

    private async Task<GrownAnalytic> InternalGetMonthlyGrownAnalytic<T>() where T : class, IEntityWithTime<int>
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var previousMonthEnd = currentMonthStart.AddTicks(-1);

        var previousCount = await _dbContext.Set<T>()
            .CountAsync(x => x.CreationDate >= previousMonthStart && x.CreationDate <= previousMonthEnd);
        var currentCount = await _dbContext.Set<T>()
            .CountAsync(x => x.CreationDate >= currentMonthStart && x.CreationDate <= now);

        return new GrownAnalytic(previousCount, currentCount, MathHelper.GetPercentageGrowth(previousCount, currentCount));
    }

    private async Task<GrownAnalytic> InternalGetYearlyGrownAnalytic<T>() where T : class, IEntityWithTime<int>
    {
        var now = DateTime.UtcNow;
        var currentYearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var previousYearStart = currentYearStart.AddYears(-1);
        var previousYearEnd = currentYearStart.AddTicks(-1);

        var previousCount = await _dbContext.Set<T>()
            .CountAsync(x => x.CreationDate >= previousYearStart && x.CreationDate <= previousYearEnd);
        var currentCount = await _dbContext.Set<T>()
            .CountAsync(x => x.CreationDate >= currentYearStart && x.CreationDate <= now);

        return new GrownAnalytic(previousCount, currentCount, MathHelper.GetPercentageGrowth(previousCount, currentCount));
    }

    public async Task<GrownAnalytic> GetGrownAnalytic<T>(AnalyticPeriod period) where T : class, IEntityWithTime<int>
    {
        return period switch
        {
            AnalyticPeriod.Daily => await InternalGetDailyGrownAnalytic<T>(),
            AnalyticPeriod.Weekly => await InternalGetWeeklyGrownAnalytic<T>(),
            AnalyticPeriod.Monthly => await InternalGetMonthlyGrownAnalytic<T>(),
            AnalyticPeriod.Yearly => await InternalGetYearlyGrownAnalytic<T>(),
            _ => new GrownAnalytic(0, 0, 0),
        };
    }

    public async Task<List<ApplicationUser>> GetUsers(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public record struct GrownAnalytic(int PerviousCount, int CurrentCount, double PercentageDifferent);
}
