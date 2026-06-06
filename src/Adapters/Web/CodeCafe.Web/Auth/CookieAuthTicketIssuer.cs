using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CodeCafe.Web.Auth;

/// <summary>
/// Thin helper that turns a successful login result into the claims
/// principal the cookie middleware expects, and signs it in. Kept in the
/// Web adapter because it is the only layer allowed to know about cookies
/// and HTTP context.
/// </summary>
public sealed class CookieAuthTicketIssuer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieAuthTicketIssuer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Issue a cookie-based identity for the given user. The display name
    /// claim is optional; the controller decides whether to include it.
    /// </summary>
    public Task SignInAsync(Guid userId, string email, string? displayName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
        };

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            claims.Add(new Claim(ClaimTypes.Name, displayName));
        }

        var identity = new ClaimsIdentity(claims, WebAuthExtensions.CookieScheme);
        var principal = new ClaimsPrincipal(identity);

        var http = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HTTP context to sign in to.");
        return http.SignInAsync(WebAuthExtensions.CookieScheme, principal);
    }

    /// <summary>Sign the current user out and clear the cookie.</summary>
    public Task SignOutAsync()
    {
        var http = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HTTP context to sign out of.");
        return http.SignOutAsync(WebAuthExtensions.CookieScheme);
    }
}
