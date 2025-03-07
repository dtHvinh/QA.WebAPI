using WebAPI.Model;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;

namespace WebAPI.Repositories.Base;

public interface IAdminRepository : IRepository<AppUser>
{
    Task<AdminRepository.GrownAnalytic> GetGrownAnalytic<T>(Enums.AnalyticPeriod period) where T : class, IEntityWithTime<int>;
}