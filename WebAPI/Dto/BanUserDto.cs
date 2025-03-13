namespace WebAPI.Dto;

public class BanUserDto
{
    public int Days { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Reason { get; set; } = string.Empty;
}
