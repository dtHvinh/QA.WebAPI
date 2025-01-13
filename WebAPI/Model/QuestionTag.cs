using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

[PrimaryKey(nameof(QuestionId), nameof(TagId))]
public class QuestionTag : IKeylessEntityWithTime
{
    [Key, Column(Order = 0), ForeignKey(nameof(Question))]
    public Guid QuestionId { get; set; }
    [Key, Column(Order = 1), ForeignKey(nameof(Tag))]
    public Guid TagId { get; set; }

    public Question? Question { get; set; }
    public Tag? Tag { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
