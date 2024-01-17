namespace OpenWorld.Data.Users.Models;
public record User(int Id, string Username, string PasswordHash, string Role)
{
    public User() : this(0, "", "", "")
    {
    }
}
