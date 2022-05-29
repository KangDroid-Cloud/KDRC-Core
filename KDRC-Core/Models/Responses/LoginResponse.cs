using System.ComponentModel;

namespace KDRC_Core.Models.Responses;

public class LoginResponse
{
    public string Token { get; set; }

    public DateTimeOffset ValidUntil { get; set; }
}