using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace CodeCafe.Web.Auth;

/// <summary>
/// HTTP-level authentication wiring. The actual user identity lives in the
/// Platform module; this file only knows how to issue and validate the
/// cookie the browser carries.
/// </summary>
public static class WebAuthExtensions
{
    /// <summary>
    /// The single scheme name used across the Web adapter. Keeping it as a
    /// constant means handlers and controllers cannot drift apart.
    /// </summary>
    public const string CookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    /// <summary>
    /// Configure cookie authentication. Policies are minimal: protected
    /// endpoints require a non-anonymous identity. We do not configure
    /// sliding expiration or external providers here.
    /// </summary>
    public static IServiceCollection AddCodeCafeWebAuth(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieScheme)
            .AddCookie(CookieScheme, options =>
            {
                options.Cookie.Name = "codecafe.auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;

                // We always answer JSON from the API, so there is no login
                // page to redirect to. Returning 401/403 is more honest for
                // a JSON-only surface.
                options.Events.OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder(CookieScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    /// <summary>
    /// Activate the auth middleware. Must be called after routing and
    /// before <c>UseAuthorization</c> on the request pipeline.
    /// </summary>
    public static IApplicationBuilder UseCodeCafeWebAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
