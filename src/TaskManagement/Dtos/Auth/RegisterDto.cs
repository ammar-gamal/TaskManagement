using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos.Auth;

public class RegisterDto
{

    [StringLength(50, MinimumLength = 3)]
    [Required]

    public string Username { get; set; } = null!;

    [DataType(DataType.Password)]
    [StringLength(50, MinimumLength = 3)]
    [Required]

    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required]
    [Compare("Password", ErrorMessage = "Password dosen't match")]
    public string ConfirmPassword { get; set; } = null!;
}
