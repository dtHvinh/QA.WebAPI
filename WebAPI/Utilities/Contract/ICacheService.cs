using WebAPI.Model;

namespace WebAPI.Utilities.Contract;

public interface ICacheService
{
    Task<AppUser?> GetAppUserAsync(Guid id);
    Task<bool> IsEmailUsed(string email);
    Task RemoveAppUserAsync(Guid id);
    Task SetUsedEmail(string email);
    Task SetAppUserAsync(AppUser user);
    Task SetQuestionAsync(Question question);
    Task<Question?> GetQuestionAsync(Guid id);
}
