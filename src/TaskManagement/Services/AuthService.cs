using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Dtos.Auth;
using TaskManagement.Entities;
using TaskManagement.ExtensionMethods.Mapping;
using TaskManagement.Options;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utilites;

namespace TaskManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly TimeProvider _timeProvider;
        private readonly IAppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IOptions<JwtOptions> jwtOptions,
            TimeProvider timeProvider,
            IAppDbContext context,
            ILogger<AuthService> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _timeProvider = timeProvider;
            _context = context;
            _logger = logger;
        }
        public async Task<Result> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Registration attempt for username '{Username}'.",
                registerDto.Username);

            var usernameExists = await _context.Users.AnyAsync(e => e.Username == registerDto.Username, ct);
            if (usernameExists)
            {
                _logger.LogWarning("Registration failed. Username '{Username}' is already taken.",
                    registerDto.Username);

                return Error.Conflict("Username is already taken.");
            }


            var user = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("User '{Username}' registered successfully with Id {UserId}.",
                user.Username,
                user.Id);

            return Result.Ok();
        }
        public async Task<Result<JwtDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Login attempt for username '{Username}'.",
                loginDto.Username);

            //SQL Server is case-insensitive no need for normalizations
            var user = await _context.Users.AsNoTracking()
                                           .FirstOrDefaultAsync(u => u.Username == loginDto.Username, ct);

            if (user == null)
            {
                _logger.LogWarning("Login failed. Username '{Username}' was not found.",
                    loginDto.Username);

                return Error.Unauthorized("Invalid username or password.");
            }
            bool isPasswordValid = string.Equals(loginDto.Password, user.Password, StringComparison.Ordinal);


            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed. Invalid password for user '{Username}' (Id: {UserId}).",
                    user.Username,
                    user.Id);

                return Error.Unauthorized("Invalid username or password.");
            }

            _logger.LogInformation("User '{Username}' (Id: {UserId}) logged in successfully.",
                user.Username,
                user.Id);

            return GenerateToken(user).ToDto();

        }


        private JwtSecurityToken GenerateToken(User user)
        {
            List<Claim> claims = new()
            {
                new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new(ClaimTypes.Name,user.Username!),

            };
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: _timeProvider.GetUtcNow().AddMinutes(_jwtOptions.TokenExpirationInMinutes).UtcDateTime,
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}
