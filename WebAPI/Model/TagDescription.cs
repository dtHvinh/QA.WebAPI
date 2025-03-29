using Microsoft.EntityFrameworkCore;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[Index(nameof(TagId))]
public class TagDescription : IEntity<int>
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;

    public int TagId { get; set; }
    public Tag? Tag { get; set; }

}
