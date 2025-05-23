using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class CollectionLike : IEntityWithTime<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModificationDate { get; set; }

    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    [ForeignKey(nameof(Collection))]
    public int CollectionId { get; set; }
    public Collection? Collection { get; set; }
}
