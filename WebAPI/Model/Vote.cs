using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Vote : IEntity<int>, IOwnedByUser<int>
{
    public int Id { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public AppUser? Author { get; set; }

    public bool IsUpvote { get; set; }

    public string? VoteType { get; set; }
}

public enum VoteTypes
{
    Answer,
    Question
}