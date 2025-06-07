namespace CVSWithLibrary;

public class AuthService
{
    private readonly string _userFile = "Users.txt";
    private readonly string _logFile = "log.txt";

    public Dictionary<string, User> LoadUsers()
    {
        if (!File.Exists(_userFile))
            File.WriteAllText(_userFile, "admin,Admin123*,true\n");

        return File.ReadAllLines(_userFile)
            .Where(l => l.Split(',').Length == 3)
            .Select(User.FromCsv)
            .ToDictionary(u => u.Username);
    }

    public string? ShowInitialMenu(Dictionary<string, User> users)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== WELCOME ===");
            Console.WriteLine("1. Log in");
            Console.WriteLine("2. Register");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");
            var option = Console.ReadLine();

            switch (option)
            {
                case "1": return AuthenticateUser(users);
                case "2":
                    var newUser = RegisterUser(users);
                    if (newUser != null)
                    {
                        users[newUser.Username] = newUser;
                        SaveUsers(users);
                        Console.WriteLine("User registered successfully. Please log in.");
                        Thread.Sleep(1500);
                    }
                    break;
                case "0": return null;
            }
        }
    }

    public User? RegisterUser(Dictionary<string, User> users)
    {
        Console.Write("New username: ");
        var username = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(username) || users.ContainsKey(username!))
        {
            Console.WriteLine("Invalid or duplicate username.");
            return null;
        }

        Console.Write("Password: ");
        var password = Console.ReadLine();

        return new User
        {
            Username = username!,
            Password = password!,
            IsActive = true
        };
    }

    public void SaveUsers(Dictionary<string, User> users)
    {
        var lines = users.Values.Select(u => u.ToString());
        File.WriteAllLines(_userFile, lines);
    }

    public string? AuthenticateUser(Dictionary<string, User> users)
    {
        int attempts = 0;
        string? lastTried = null;

        while (attempts++ < 3)
        {
            Console.Write("Username: ");
            var u = Console.ReadLine();
            Console.Write("Password: ");
            var p = Console.ReadLine();

            if (u != null && users.TryGetValue(u, out var user))
            {
                if (!user.IsActive)
                {
                    Console.WriteLine("User is blocked.");
                    return null;
                }

                if (user.Password == p)
                    return u;

                lastTried = u;
            }
            Console.WriteLine("Invalid credentials.");
        }

        if (lastTried != null)
        {
            var lines = File.ReadAllLines(_userFile);
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts[0] == lastTried)
                {
                    lines[i] = $"{parts[0]},{parts[1]},false";
                    break;
                }
            }
            File.WriteAllLines(_userFile, lines);
            Console.WriteLine("User has been blocked.");
        }

        return null;
    }

    public void WriteLog(string type, string message, string? user)
    {
        var color = type == "ERROR" ? ConsoleColor.Red : ConsoleColor.Blue;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{type}] {message}");
        Console.ResetColor();

        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {user}: {message}";
        File.AppendAllText(_logFile, logEntry + Environment.NewLine);
    }
}