using TaskManagement.Dtos.Auth;
using TaskManagement.Utilites;

namespace TaskManagement.Services.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default);
    Task<Result<JwtDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
}
