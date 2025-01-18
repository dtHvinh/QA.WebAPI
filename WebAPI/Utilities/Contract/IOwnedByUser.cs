using WebAPI.Model;

namespace WebAPI.Utilities.Contract;

public interface IOwnedByUser<TKey> where TKey : allows ref struct
{
    TKey AuthorId { get; set; }
    AppUser? Author { get; set; }
}
