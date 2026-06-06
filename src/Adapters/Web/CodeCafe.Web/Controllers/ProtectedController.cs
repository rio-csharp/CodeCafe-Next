using CodeCafe.Modules.Platform.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCafe.Web.Controllers;

/// <summary>
/// Minimal protected endpoint whose only job is to prove the auth pipeline
/// is wired end to end. It demonstrates the ICurrentUser abstraction reaching
/// the controller without leaking HttpContext plumbing into business code.
/// </summary>
[ApiController]
[Route("api/protected")]
[Authorize]
public sealed class ProtectedController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public ProtectedController(ICurrentUser currentUser) => _currentUser = currentUser;

    [HttpGet("ping")]
    public IActionResult Ping() =>
        Ok(new
        {
            ok = true,
            userId = _currentUser.UserId,
            email = _currentUser.Email,
        });
}
