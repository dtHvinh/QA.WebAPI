using WebAPI.Model;

namespace WebAPI.Utilities.Contract;

public interface ICacheService
{
    Task<Question?> GetQuestionAsync(int id);
    Task<AppUser?> GetAppUserAsync(int id);
    Task<List<Tag>?> GetTags(string orderBy, int skip, int take);
    Task<bool> IsEmailUsed(string email);
    Task RemoveAppUserAsync(int id);
    Task SetUsedEmail(string email);
    Task SetAppUserAsync(AppUser user);
    Task SetQuestionAsync(Question question);
    Task SetTags(string orderBy, int skip, int take, List<Tag> values);
    Task SetTagDetail(Tag tag, string orderBy, int questionPage, int questionPageSize);
    Task<Tag?> GetTagWithQuestion(int tagId, string orderBy, int questionPage, int questionPageSize);
}
