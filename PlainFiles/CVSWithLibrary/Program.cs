using CVSWithLibrary;

var auth = new AuthService();
File.AppendAllText("log.txt", $"[START] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - Program started\n");
var users = auth.LoadUsers();
var activeUser = auth.ShowInitialMenu(users);

if (activeUser == null)
{
    File.AppendAllText("log.txt", $"[BLOCKED] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - Program closed due to blocked or cancelled login\n");
    Console.WriteLine("Program terminated.");
    return;
}

if (!File.Exists("people.csv"))
    File.WriteAllText("people.csv", "Id,FirstName,LastName,Phone,City,Balance\n");
if (!File.Exists("log.txt"))
    File.Create("log.txt").Close();

var helper = new CsvHelperExample();
var people = helper.Read("people.csv").ToList();

string option;
do
{
    option = ShowMenu();

    switch (option)
    {
        case "1":
            Console.WriteLine("==============================");
            int itemNumber = 1;
            foreach (var p in people)
            {
                Console.WriteLine($"Item: {itemNumber++}\n  ID: {p.Id}\n  {p.FirstName} {p.LastName}\n  Phone: {FormatPhone(p.Phone)}\n  City: {p.City}\n  Balance: {p.Balance,20:C2}\n");
            }
            break;

        case "2":
            Console.Write("Enter ID: ");
            var idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int id)) { Error("ID must be a valid number."); break; }
            if (people.Any(p => p.Id == id)) { Error($"The ID {id} already exists. Use a unique ID."); break; }

            Console.Write("Enter first name: ");
            var firstName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(firstName)) { Error("First name cannot be empty."); break; }

            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName)) { Error("Last name cannot be empty."); break; }

            Console.Write("Enter phone: ");
            var phoneInput = Console.ReadLine();
            var phoneDigits = new string(phoneInput?.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length != 10) { Error("Invalid phone: must contain exactly 10 digits. Example: 312 886 5534"); break; }

            Console.Write("Enter city: ");
            var city = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(city)) { Error("City cannot be empty."); break; }

            Console.Write("Enter balance: ");
            var balanceInput = Console.ReadLine();
            if (!decimal.TryParse(balanceInput, out decimal balance)) { Error("Balance must be a valid number."); break; }
            if (balance < 0) { Error("Balance cannot be negative."); break; }

            people.Add(new Person { Id = id, FirstName = firstName!, LastName = lastName!, Phone = phoneDigits, City = city!, Balance = balance });
            Success($"Added person with ID {id}");
            break;

        case "3":
            helper.Write("people.csv", people);
            break;

        case "4":
            Console.Write("Enter ID to edit: ");
            if (int.TryParse(Console.ReadLine(), out int editId))
            {
                var person = people.FirstOrDefault(p => p.Id == editId);
                if (person != null)
                {
                    Console.Write($"First name [{person.FirstName}]: ");
                    var newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal)) person.FirstName = newVal;

                    Console.Write($"Last name [{person.LastName}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal)) person.LastName = newVal;

                    Console.Write($"Phone [{FormatPhone(person.Phone)}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal))
                    {
                        var digits = new string(newVal.Where(char.IsDigit).ToArray());
                        if (digits.Length == 10) person.Phone = digits;
                        else { Error("Invalid phone: must contain exactly 10 digits. Example: 312 886 5534"); break; }
                    }

                    Console.Write($"City [{person.City}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal)) person.City = newVal;
                    else if (string.IsNullOrWhiteSpace(person.City)) { Error("City cannot be empty."); break; }

                    Console.Write($"Balance [{person.Balance}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal))
                    {
                        if (decimal.TryParse(newVal, out var newBal))
                        {
                            if (newBal >= 0) person.Balance = newBal;
                            else { Error("Balance cannot be negative."); break; }
                        }
                        else { Error("Balance must be a valid number."); break; }
                    }

                    Success($"Updated person with ID {editId}");
                }
                else Error("Person not found.");
            }
            break;

        case "5":
            Console.Write("Enter ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int delId))
            {
                var person = people.FirstOrDefault(p => p.Id == delId);
                if (person != null)
                {
                    Console.Write($"Are you sure you want to delete {person.FirstName} {person.LastName}? (Y/N): ");
                    if (Console.ReadLine()?.ToUpper() == "Y")
                    {
                        people.Remove(person);
                        Success($"Deleted person with ID {delId}");
                    }
                }
                else Error("Person not found.");
            }
            break;

        case "6":
            Console.Clear();
            var peopleByCity = people.OrderBy(p => p.Id).GroupBy(p => p.City).OrderBy(g => g.Key);
            int idWidth = 4, nameWidth = 12, lastWidth = 12, saldoWidth = 15;
            decimal totalGeneral = 0;

            foreach (var group in peopleByCity)
            {
                Console.WriteLine($"Ciudad: {group.Key}\n");
                Console.WriteLine($"{"ID".PadRight(idWidth)} {"Nombres".PadRight(nameWidth)} {"Apellidos".PadRight(lastWidth)} {"Saldo".PadLeft(saldoWidth)}");
                Console.WriteLine($"{new string('-', idWidth)} {new string('-', nameWidth)} {new string('-', lastWidth)} {new string('-', saldoWidth)}");

                decimal subtotal = 0;
                foreach (var p in group)
                {
                    Console.WriteLine($"{p.Id.ToString().PadRight(idWidth)} {p.FirstName.PadRight(nameWidth)} {p.LastName.PadRight(lastWidth)} {FormatSaldo(p.Balance)}");
                    subtotal += p.Balance;
                }

                Console.WriteLine($"{new string(' ', idWidth + nameWidth + lastWidth + 3)}{new string('=', saldoWidth)}");
                Console.WriteLine($"Total: {group.Key.PadRight(idWidth + nameWidth + lastWidth + 3)}{FormatSaldo(subtotal)}\n");
                totalGeneral += subtotal;
            }
            Console.WriteLine($"{new string(' ', idWidth + nameWidth + lastWidth + 3)}{new string('=', saldoWidth)}");
            Console.WriteLine($"Total General:{"".PadRight(idWidth + nameWidth + lastWidth - 6)}{FormatSaldo(totalGeneral)}\n");
            break;

        case "0":
            helper.Write("people.csv", people);
            Success("Saved all changes to CSV");
            File.AppendAllText("log.txt", $"[END] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - Program ended by user {activeUser}\n");
            break;
    }

} while (option != "0");

string ShowMenu()
{
    Console.WriteLine("\n==============================");
    Console.WriteLine("1. Show content");
    Console.WriteLine("2. Add person");
    Console.WriteLine("3. Save changes");
    Console.WriteLine("4. Edit person");
    Console.WriteLine("5. Delete person");
    Console.WriteLine("6. Report");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");
    return Console.ReadLine() ?? "0";
}

string FormatPhone(string phone)
{
    var digits = new string(phone.Where(char.IsDigit).ToArray());
    return digits.Length == 10 ? $"{digits[..3]} {digits[3..6]} {digits[6..]}" : phone;
}

string FormatSaldo(decimal value)
{
    var saldoWidth = 15;
    var formatted = value.ToString("N2");
    return formatted.Length > saldoWidth ? formatted[^saldoWidth..] : formatted.PadLeft(saldoWidth);
}

void Error(string msg)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {msg}");
    Console.ResetColor();
}

void Success(string msg)
{
    File.AppendAllText("log.txt", $"[SUCCESS] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - User: {activeUser} -> {msg}\n");
}
