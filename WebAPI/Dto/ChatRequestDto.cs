namespace WebAPI.Dto;

public class ChatRequestDto
{
    public int ChatRoomId { get; set; }
    public string Message { get; set; } = default!;
    public List<IFormFile>? Files { get; set; } = default!;
}
