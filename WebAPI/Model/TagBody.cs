using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class TagBody : IEntity<int>
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    [ForeignKey(nameof(Tag))]
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
