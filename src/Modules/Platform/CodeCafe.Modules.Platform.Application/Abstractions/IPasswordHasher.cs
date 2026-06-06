namespace CodeCafe.Modules.Platform.Application.Abstractions;

/// <summary>
/// Password hashing contract. The implementation lives in infrastructure
/// because it is tied to a specific algorithm and a specific random-source
/// policy. Handlers depend on this abstraction, not on a concrete hasher.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash the plaintext into an opaque, salted, self-describing string.
    /// </summary>
    string Hash(string plaintext);

    /// <summary>
    /// Constant-time comparison of a plaintext against a stored hash.
    /// Returns <c>true</c> only if the plaintext matches.
    /// </summary>
    bool Verify(string plaintext, string hash);
}
