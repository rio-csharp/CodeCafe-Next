namespace CodeCafe.Modules.Platform.Application.Exceptions;

/// <summary>
/// Raised when login fails because the email is unknown or the password does
/// not match. The message intentionally stays generic so the API does not
/// leak which side was wrong.
/// </summary>
public sealed class InvalidCredentialsException : AuthException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.") { }
}
