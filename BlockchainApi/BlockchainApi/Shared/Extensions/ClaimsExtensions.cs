using System.Security.Claims;

namespace Backend.Shared.Extensions;

public static class ClaimsExtension
{
    public static Guid? GetId(this ClaimsPrincipal principal)
    {
        var nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(nameIdentifier) && Guid.TryParse(nameIdentifier, out var id))
        {
            return id;
        }
        return null;
    }
}
