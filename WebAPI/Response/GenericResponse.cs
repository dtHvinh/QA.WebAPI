namespace WebAPI.Response;

public record GenericResponse(string Message = "Successfully")
{
    public static implicit operator GenericResponse(string Message) => new(Message);
}
