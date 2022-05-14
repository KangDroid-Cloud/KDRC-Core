using System.Security.Cryptography;
using System.Text;

namespace KDRC_Core.Models.Data;

public class AccessToken
{
    /// <summary>
    /// Custom-Generated ID(This works as token)
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Owner of this access token
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Defines when this token is expired.(Roughly about 20m)
    /// </summary>
    public DateTimeOffset ValidUntil { get; set; }

    public static AccessToken CreateDefault(string userId)
    {
        using var shaManaged = SHA512.Create();
        var targetString = $"{DateTime.UtcNow.Ticks}/{userId}/{Guid.NewGuid().ToString()}";
        var targetByte = Encoding.UTF8.GetBytes(targetString);
        var result = shaManaged.ComputeHash(targetByte);

        return new AccessToken
        {
            Id = BitConverter.ToString(result).Replace("-", string.Empty),
            UserId = userId,
            ValidUntil = DateTimeOffset.UtcNow.AddMinutes(20)
        };
    }
}