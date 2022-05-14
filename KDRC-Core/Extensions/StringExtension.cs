using System.Diagnostics.CodeAnalysis;

namespace KDRC_Core.Extensions;

[ExcludeFromCodeCoverage]
public static class StringExtension
{
    /// <summary>
    /// Check Hashed Password is actually correct password or not.
    /// </summary>
    /// <param name="hashedPassword">Extension String(Target) The hashed Password</param>
    /// <param name="plainPassword">Input - Plaintext password(probably from requestModel)</param>
    /// <returns>true if password matches, false if password does not matches.</returns>
    public static bool CheckHashedPassword(this string hashedPassword, string plainPassword)
    {
        var correct = false;
        try
        {
            correct = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
        catch
        {
        }

        return correct;
    }
}