namespace CodeCafe.Modules.Platform.Application.Exceptions;

/// <summary>
/// Raised when registration tries to use an email that already exists.
/// </summary>
public sealed class EmailAlreadyExistsException : AuthException
{
    public EmailAlreadyExistsException()
        : base("An account with that email already exists.") { }
}
