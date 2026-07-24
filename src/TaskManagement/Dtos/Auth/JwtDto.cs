namespace TaskManagement.Dtos.Auth;

public class JwtDto
{
    public string Token { get; set; } = null!;
    public DateTime TokenExpiration { get; set; }
}
