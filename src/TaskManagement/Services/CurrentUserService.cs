using TaskManagement.ExtensionMethods;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public int Id
    {
        get
        {
            var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available. CurrentUser can only be used during an active HTTP request.");
            var userId = context.User.GetId();

            return userId is null
                ? throw new UnauthorizedAccessException("Principal is missing the required User ID claim. Ensure the endpoint is decorated with [Authorize].")
                : userId.Value;
        }
    }

}
