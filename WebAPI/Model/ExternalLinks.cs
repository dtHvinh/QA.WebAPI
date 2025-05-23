using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class ExternalLinks : IEntity<int>, IOwnedByUser<int>
{
    public int Id { get; set; }
    [ForeignKey(nameof(Author))] public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }
    public string Provider { get; set; } = null!;
    public string Url { get; set; } = null!;
}