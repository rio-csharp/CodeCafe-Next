using System.Security.Cryptography;
using CodeCafe.Modules.Platform.Application.Abstractions;

namespace CodeCafe.Modules.Platform.Infrastructure.Auth;

/// <summary>
/// Password hasher built on PBKDF2 (Rfc2898DeriveBytes with SHA-256).
/// The output is self-describing: <c>PBKDF2$&lt;iterations&gt;$&lt;saltBase64&gt;$&lt;hashBase64&gt;</c>.
/// Keeping the iteration count inside the stored hash lets us raise it
/// over time without a forced password reset migration.
///
/// We chose PBKDF2 over rolling our own scheme because it is part of the
/// BCL, has no extra NuGet dependency, and is the same primitive the
/// ASP.NET Core identity stack historically defaulted to.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;             // 128 bits
    private const int HashSize = 32;             // 256 bits
    private const int Iterations = 100_000;      // ~100ms on modern hardware
    private const string Algorithm = "PBKDF2";
    private const char Separator = '$';

    public string Hash(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            throw new ArgumentException("Password is required.", nameof(plaintext));
        }

        Span<byte> salt = stackalloc byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            plaintext,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return string.Join(
            Separator,
            Algorithm,
            Iterations.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string plaintext, string hash)
    {
        if (string.IsNullOrEmpty(plaintext) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        var parts = hash.Split(Separator);
        if (parts.Length != 4 || parts[0] != Algorithm)
        {
            return false;
        }

        if (!int.TryParse(parts[1], System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var iterations) || iterations <= 0)
        {
            return false;
        }

        byte[] salt, expected;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expected = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actual = Rfc2898DeriveBytes.Pbkdf2(
            plaintext,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expected.Length);

        // Constant-time comparison.
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
