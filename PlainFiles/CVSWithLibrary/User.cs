namespace CVSWithLibrary;

public class User
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; }

    public override string ToString()
    {
        return $"{Username},{Password},{IsActive}";
    }

    public static User FromCsv(string line)
    {
        var parts = line.Split(',');
        return new User
        {
            Username = parts[0],
            Password = parts[1],
            IsActive = bool.Parse(parts[2])
        };
    }
}