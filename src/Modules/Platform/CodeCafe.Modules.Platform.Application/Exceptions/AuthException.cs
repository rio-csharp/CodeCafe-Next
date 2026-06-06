namespace CodeCafe.Modules.Platform.Application.Exceptions;

/// <summary>
/// Base type for authentication-flow exceptions raised by handlers. Adapters
/// map known subclasses to stable HTTP status codes; unknown exceptions flow
/// to the global handler as 500s.
/// </summary>
public abstract class AuthException : Exception
{
    protected AuthException(string message) : base(message) { }
}
