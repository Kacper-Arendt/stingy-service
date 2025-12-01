using Microsoft.AspNetCore.Identity;

namespace Auth.Core.Entities;

public class User : IdentityUser
{
    public static User CreateNormalUser(string email)
    {
        var user = new User();
        user.Initialize(email, email);
        return user;
    }

    private void Initialize(string email, string name)
    {
        Email = email;
        UserName = name;
    }
}