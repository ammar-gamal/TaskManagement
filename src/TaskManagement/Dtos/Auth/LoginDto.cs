using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos.Auth;

public class LoginDto
{
    public string Username { get; set; } = null!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
