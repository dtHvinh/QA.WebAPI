namespace WebAPI.Dto;

public class UpdateCommunityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public IFormFile? IconImage { get; set; }
    public bool IsPrivate { get; set; }
}
