using WebAPI.Model;

namespace WebAPI.Utilities.Contract;

public interface ICacheService
{
    Task<Question?> GetQuestionAsync(Guid id);
    Task<AppUser?> GetAppUserAsync(Guid id);
    Task<List<Tag>?> GetTags(string orderBy, int skip, int take);
    Task<bool> IsEmailUsed(string email);
    Task RemoveAppUserAsync(Guid id);
    Task SetUsedEmail(string email);
    Task SetAppUserAsync(AppUser user);
    Task SetQuestionAsync(Question question);
    Task SetTags(string orderBy, int skip, int take, List<Tag> values);
    Task SetTagDetail(Tag tag);
    Task<Tag?> GetTagDetail(Guid tagId);
}
