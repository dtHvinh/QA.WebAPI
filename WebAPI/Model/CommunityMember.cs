using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class CommunityMember : IEntity<int>
{
    public int Id { get; set; }

    [ForeignKey(nameof(Community))]
    public int CommunityId { get; set; }
    public Community Community { get; set; } = default!;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;

    public bool IsOwner { get; set; }
    public bool IsModerator { get; set; }
}
