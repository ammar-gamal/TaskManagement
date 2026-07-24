namespace TaskManagement.Options;

public class JwtOptions
{
    public static string SectionName => "Jwt";
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public double TokenExpirationInMinutes { get; set; }
}
