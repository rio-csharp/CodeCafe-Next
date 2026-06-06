using System.Security.Claims;
using CodeCafe.Modules.Platform.Application.Abstractions;

namespace CodeCafe.Web.Auth;

/// <summary>
/// HTTP-backed <see cref="ICurrentUser"/>. Reads the user id from the
/// authentication principal the cookie middleware attached. Anonymous
/// requests surface as <see cref="IsAuthenticated"/> = <c>false</c> and a
/// null id.
///
/// Lives in the Web adapter because it depends on
/// <see cref="IHttpContextAccessor"/>; the application layer never sees it.
/// </summary>
public sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var principal = Principal;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var raw = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public string? Email =>
        IsAuthenticated ? Principal?.FindFirstValue(ClaimTypes.Email) : null;
}
