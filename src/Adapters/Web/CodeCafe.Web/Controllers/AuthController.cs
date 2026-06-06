using CodeCafe.Modules.Platform.Application.Auth.Commands.Login;
using CodeCafe.Modules.Platform.Application.Auth.Commands.Register;
using CodeCafe.Modules.Platform.Application.Auth.Queries.GetCurrentUser;
using CodeCafe.Modules.Platform.Contracts.Auth;
using CodeCafe.Web.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCafe.Web.Controllers;

/// <summary>
/// Thin HTTP surface for the Platform authentication use cases. Each action
/// binds the request, calls the corresponding application handler through
/// MediatR, and maps the result to a transport shape. No business rules live
/// in this file.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly CookieAuthTicketIssuer _ticketIssuer;

    public AuthController(IMediator mediator, CookieAuthTicketIssuer ticketIssuer)
    {
        _mediator = mediator;
        _ticketIssuer = ticketIssuer;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.Email, request.Password, request.DisplayName),
            cancellationToken);

        return Ok(new AuthResponse(result.UserId, result.Email, result.DisplayName));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        await _ticketIssuer.SignInAsync(result.UserId, result.Email, result.DisplayName);
        return Ok(new AuthResponse(result.UserId, result.Email, result.DisplayName));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _ticketIssuer.SignOutAsync();
        return NoContent();
    }

    /// <summary>
    /// Current-user endpoint. Doubles as a proof-of-authentication check:
    /// a 401 here means the cookie did not carry a valid identity.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserResponse>> Me(CancellationToken cancellationToken)
    {
        var view = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        if (view is null)
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse(view.UserId, view.Email, view.DisplayName));
    }
}
