using System.Security.Claims;

namespace TaskManagement.ExtensionMethods;

public static class ClaimPrincipleExtensions
{
    public static int? GetId(this ClaimsPrincipal claimsPrincipal) =>
        int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : null;
}
