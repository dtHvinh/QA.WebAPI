namespace WebAPI.Response;

public record TextResponse(string Message = "Successfully")
{
    public static implicit operator TextResponse(string Message) => new(Message);
}
