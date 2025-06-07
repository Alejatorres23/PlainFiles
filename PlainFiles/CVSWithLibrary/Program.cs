// Versión simplificada del proyecto CVSWithLibrary
using CVSWithLibrary;
using System.Text;

var usuarios = CargarUsuarios("Users.txt");
var usuarioActivo = AutenticarUsuario(usuarios);
if (usuarioActivo == null)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Access denied. User blocked.");
    Console.ResetColor();
    return;
}

if (!File.Exists("people.csv"))
    File.WriteAllText("people.csv", "Id,FirstName,LastName,Phone,City,Balance\n");
if (!File.Exists("log.txt"))
    File.Create("log.txt").Close();

var helper = new CsvHelperExample();
var personas = helper.Read("people.csv").ToList();

string opcion;
do
{
    opcion = Menu();
    Log("INFO", $"Option selected: {opcion}");
    Console.WriteLine("==========================");

    switch (opcion)
    {
        case "1":
            Log("INFO", "Show content");
            if (!personas.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Empty file.");
                Console.ResetColor();
                Log("ERROR", "Empty file.");
                break;
            }
            personas.ForEach(p => Console.WriteLine(p));
            break;

        case "2":
            Log("INFO", "Add person");
            Console.Write("ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id) || personas.Any(p => p.Id == id))
            {
                Log("ERROR", "Invalid ID");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ID must be a number and unique.");
                Console.ResetColor();
                break;
            }
            var persona = CapturarPersona(id);
            if (persona != null)
                personas.Add(persona);
            break;

        case "3":
            Log("INFO", "Save changes");
            helper.Write("people.csv", personas);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Changes saved.");
            Console.ResetColor();
            break;

        case "4":
            Log("INFO", "Edit person");
            Console.Write("Enter ID to edit: ");
            if (int.TryParse(Console.ReadLine(), out int idEdit))
            {
                var p = personas.FirstOrDefault(x => x.Id == idEdit);
                if (p != null)
                {
                    Console.Write($"First name [{p.FirstName}]: "); var nuevo = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(nuevo)) p.FirstName = nuevo;
                    Console.Write($"Last name [{p.LastName}]: "); nuevo = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(nuevo)) p.LastName = nuevo;
                    Console.Write($"Phone [{p.Phone}]: "); nuevo = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(nuevo)) { var tmp = FormatearTelefono(nuevo); if (EsTelefonoValido(tmp)) p.Phone = tmp; }
                    Console.Write($"City [{p.City}]: "); nuevo = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(nuevo)) p.City = nuevo;
                    Console.Write($"Balance [{p.Balance}]: "); nuevo = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(nuevo) && decimal.TryParse(nuevo, out var b) && b >= 0) p.Balance = b;
                }
            }
            break;

        case "5":
            Log("INFO", "Delete person");
            Console.Write("Enter ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int idDel))
            {
                var p = personas.FirstOrDefault(x => x.Id == idDel);
                if (p != null)
                {
                    Console.Write($"Delete {p.FirstName} {p.LastName}? (Y/N): ");
                    if (Console.ReadLine()?.ToUpper() == "Y")
                    {
                        personas.Remove(p);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Deleted.");
                        Console.ResetColor();
                    }
                }
            }
            break;

        case "6":
            Log("INFO", "City report");
            decimal total = 0;
            foreach (var grupo in personas.GroupBy(p => p.City))
            {
                Console.WriteLine($"\nCiudad: {grupo.Key}\n");
                Console.WriteLine("ID  Nombres       Apellidos     Saldo");
                Console.WriteLine("--  ------------- ------------- ----------");

                decimal sub = 0;
                foreach (var p in grupo)
                {
                    Console.WriteLine($"{p.Id,-3}{p.FirstName,-14}{p.LastName,-14}{p.Balance,10:N2}");
                    sub += p.Balance;
                }

                Console.WriteLine("=======");
                Console.WriteLine($"Total: {grupo.Key}: {sub:N2}\n");
                total += sub;
            }
            Console.WriteLine("=======");
            Console.WriteLine("Total General: " + total.ToString("N2"));
            break;
    }
} while (opcion != "0");

string Menu()
{
    Console.WriteLine("============================");
    Console.WriteLine("1. View people\n2. Add person\n3. Save changes\n4. Edit person\n5. Delete person\n6. City report");
    Console.WriteLine("0. Exit\nChoose an option: ");
    return Console.ReadLine() ?? "0";
}

void Log(string tipo, string mensaje)
{
    var color = tipo == "ERROR" ? ConsoleColor.Red : ConsoleColor.Blue;
    Console.ForegroundColor = color;
    Console.WriteLine($"[{tipo}] {mensaje}");
    Console.ResetColor();
    File.AppendAllText("log.txt", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{tipo}] {usuarioActivo}: {mensaje}\n");
}

Person? CapturarPersona(int id)
{
    Console.Write("First name: "); var nombre = Console.ReadLine();
    Console.Write("Last name: "); var apellido = Console.ReadLine();
    Console.Write("Phone: "); var telefono = Console.ReadLine();
    telefono = FormatearTelefono(telefono);
    Console.Write("City: "); var ciudad = Console.ReadLine();
    Console.Write("Balance: "); var saldoInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) ||
        !EsTelefonoValido(telefono) ||
        !decimal.TryParse(saldoInput, out decimal saldo) || saldo < 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Invalid input. All fields must be completed and valid.\n----------------------------\n");
        Console.ResetColor();
        return null;
    }

    return new Person { Id = id, FirstName = nombre, LastName = apellido, Phone = telefono, City = ciudad, Balance = saldo };
}

bool EsTelefonoValido(string telefono)
{
    string soloDigitos = new string(telefono.Where(char.IsDigit).ToArray());
    return soloDigitos.Length == 10 && soloDigitos.All(char.IsDigit);
}

string FormatearTelefono(string telefono)
{
    string soloDigitos = new string(telefono.Where(char.IsDigit).ToArray());
    return soloDigitos.Length == 10 ? $"{soloDigitos[..3]} {soloDigitos[3..6]} {soloDigitos[6..]}" : telefono;
}

Dictionary<string, (string pass, bool active)> CargarUsuarios(string ruta)
{
    if (!File.Exists(ruta)) File.WriteAllText(ruta, "admin,Admin123*,true\n");
    return File.ReadAllLines(ruta)
        .Select(l => l.Split(',')).Where(p => p.Length == 3)
        .ToDictionary(p => p[0], p => (p[1], bool.Parse(p[2])));
}

string? AutenticarUsuario(Dictionary<string, (string pass, bool active)> usuarios)
{
    int intentos = 0; string? ultimo = null;
    while (intentos++ < 3)
    {
        Console.Write("Username: "); var u = Console.ReadLine();
        Console.Write("Password: "); var p = Console.ReadLine();

        if (u != null && usuarios.TryGetValue(u, out var data))
        {
            if (!data.active) { Console.WriteLine("User blocked."); return null; }
            if (data.pass == p) return u;
            ultimo = u;
        }
        Console.WriteLine("Invalid credentials.");
    }

    if (ultimo != null)
    {
        var lineas = File.ReadAllLines("Users.txt");
        for (int i = 0; i < lineas.Length; i++)
        {
            var p = lineas[i].Split(',');
            if (p.Length == 3 && p[0] == ultimo)
            {
                lineas[i] = $"{p[0]},{p[1]},false";
                break;
            }
        }
        File.WriteAllLines("Users.txt", lineas);
        Console.WriteLine("User has been blocked.");
    }
    return null;
}
