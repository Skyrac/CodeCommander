using Backend.Shared.Extensions;
using Backend.Shared.Interfaces;

namespace Backend.Shared;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id => GetId();

    private Guid? GetId()
    {
        var id = _httpContextAccessor.HttpContext?.User?.GetId();
        return id;
    }
}
