using System.Text.RegularExpressions;

namespace KDRC_Core.Models.Requests;

public class AccountRegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string NickName { get; set; }

    public bool ValidateModel()
    {
        var emailRegex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
        if (!emailRegex.IsMatch(Email))
        {
            return false;
        }

        if (Password.Length < 8) return false;

        return true;
    }

    public Account ToAccount() => new Account
    {
        Id = Ulid.NewUlid().ToString(),
        AccountState = AccountState.Created,
        UserEmail = this.Email,
        UserNickName = this.NickName,
        UserPassword = BCrypt.Net.BCrypt.HashPassword(this.Password),
        JwtMap = new Dictionary<string, string>(),
        AccountRoles = new HashSet<AccountRole>
        {
            AccountRole.User
        }
    };
}