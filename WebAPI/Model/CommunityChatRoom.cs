using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class CommunityChatRoom : IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    [ForeignKey(nameof(Community))]
    public int CommunityId { get; set; }
    public Community Community { get; set; } = default!;

    public ICollection<ChatRoomMessage> Messages { get; set; } = [];
}
