using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Community : IEntityWithTime<int>
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
    public string? IconImage { get; set; }

    public bool IsPrivate { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public int MemberCount { get; set; } = 0;

    [JsonIgnore]
    public ICollection<CommunityMember> Members { get; set; } = default!;
    [JsonIgnore]
    public ICollection<CommunityChatRoom> Rooms { get; set; } = default!;
}

