using CVSWithLibrary;

var auth = new AuthService();
var users = auth.LoadUsers();
var activeUser = auth.ShowInitialMenu(users);

if (activeUser == null)
{
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
            foreach (var p in people.OrderBy(p => p.Id))
            {
                Console.WriteLine($"{p.Id,2}  {p.FirstName} {p.LastName}");
                Console.WriteLine($"     Phone: {FormatPhone(p.Phone)}");
                Console.WriteLine($"     City: {p.City}");
                Console.WriteLine($"     Balance: {p.Balance,20:C2}\n");
            }
            break;

        case "2":
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id) || people.Any(p => p.Id == id))
            {
                Console.WriteLine("Invalid or duplicate ID.");
                break;
            }

            Console.Write("Enter first name: ");
            var firstName = Console.ReadLine();
            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine();

            Console.Write("Enter phone: ");
            var phone = Console.ReadLine();
            phone = new string(phone?.Where(char.IsDigit).ToArray());

            Console.Write("Enter city: ");
            var city = Console.ReadLine();

            Console.Write("Enter balance: ");
            var balanceInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                phone.Length != 10 || !phone.All(char.IsDigit) ||
                !decimal.TryParse(balanceInput, out decimal balance) || balance < 0)
            {
                Console.WriteLine("Invalid input. Make sure all fields are correct.");
                break;
            }

            people.Add(new Person
            {
                Id = id,
                FirstName = firstName!,
                LastName = lastName!,
                Phone = phone!,
                City = city!,
                Balance = balance
            });

            Console.WriteLine("Person added successfully.");
            break;

        case "3":
            helper.Write("people.csv", people);
            Console.WriteLine("Changes saved successfully.");
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

                    Console.Write($"Phone [{person.Phone}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal))
                    {
                        var digits = new string(newVal.Where(char.IsDigit).ToArray());
                        if (digits.Length == 10) person.Phone = digits;
                    }

                    Console.Write($"City [{person.City}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal)) person.City = newVal;

                    Console.Write($"Balance [{person.Balance}]: ");
                    newVal = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newVal) && decimal.TryParse(newVal, out var newBal) && newBal >= 0)
                        person.Balance = newBal;

                    Console.WriteLine("Person updated successfully.");
                }
                else Console.WriteLine("Person not found.");
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
                        Console.WriteLine("Person deleted successfully.");
                    }
                }
                else Console.WriteLine("Person not found.");
            }
            break;

        case "6":
            Console.Clear();
            var col1 = "ID";
            var col2 = "Nombres";
            var col3 = "Apellidos";
            var col4 = "Saldo";

            var idWidth = 4;
            var nameWidth = 10;
            var lastWidth = 10;
            var saldoWidth = 15;

            string FormatSaldo(decimal value)
            {
                var formatted = value.ToString("N2");
                return formatted.Length > saldoWidth ? formatted[^saldoWidth..] : formatted.PadLeft(saldoWidth);
            }

            var peopleByCity = people.OrderBy(p => p.Id).GroupBy(p => p.City).OrderBy(g => g.Key);
            decimal totalGeneral = 0;

            foreach (var group in peopleByCity)
            {
                Console.WriteLine($"Ciudad: {group.Key}\n");

                Console.WriteLine($"{col1.PadRight(idWidth)} {col2.PadRight(nameWidth)} {col3.PadRight(lastWidth)} {col4.PadLeft(saldoWidth)}");
                Console.WriteLine($"{new string('-', idWidth)} {new string('-', nameWidth)} {new string('-', lastWidth)} {new string('-', saldoWidth)}");

                decimal subtotal = 0;

                foreach (var p in group)
                {
                    Console.WriteLine($"{p.Id.ToString().PadRight(idWidth)} {p.FirstName.PadRight(nameWidth)} {p.LastName.PadRight(lastWidth)} {FormatSaldo(p.Balance)}");
                    subtotal += p.Balance;
                }

                Console.WriteLine($"{new string(' ', idWidth + nameWidth + lastWidth + 3)}{new string('=', saldoWidth)}");
                Console.WriteLine($"Total: {group.Key.PadRight(idWidth + nameWidth + lastWidth + 3)}{FormatSaldo(subtotal).PadLeft(saldoWidth)}\n");

                totalGeneral += subtotal;
            }

            Console.WriteLine($"{new string(' ', idWidth + nameWidth + lastWidth + 3)}{new string('=', saldoWidth)}");
            Console.WriteLine($"Total General:{"".PadRight(idWidth + nameWidth + lastWidth - 6)}{FormatSaldo(totalGeneral).PadLeft(saldoWidth)}\n");
            break;

        case "0":
            helper.Write("people.csv", people);
            Console.WriteLine("Changes saved before exiting. Goodbye!");
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
