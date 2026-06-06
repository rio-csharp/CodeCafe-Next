using System.Text.RegularExpressions;

namespace CodeCafe.Modules.Platform.Domain.ValueObjects;

/// <summary>
/// Email value object. Normalises to lower-case and applies a deliberately
/// conservative shape check at the domain boundary. We do not own a full
/// RFC 5322 parser here: the goal is to fail fast on obvious garbage, not
/// to make authoritative deliverability claims.
/// </summary>
public sealed record Email
{
    private static readonly Regex Shape =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new ArgumentException("Email is required.", nameof(raw));
        }

        var trimmed = raw.Trim().ToLowerInvariant();
        if (!Shape.IsMatch(trimmed))
        {
            throw new ArgumentException("Email format is invalid.", nameof(raw));
        }

        return new Email(trimmed);
    }

    public override string ToString() => Value;
}
