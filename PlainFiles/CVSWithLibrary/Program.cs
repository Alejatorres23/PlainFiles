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
    auth.WriteLog("INFO", $"Option selected: {option}", activeUser);

    switch (option)
    {
        case "1":
            Console.WriteLine("==========================");
            if (!people.Any())
            {
                Console.WriteLine("No records found.");
                break;
            }

            foreach (var p in people)
            {
                Console.WriteLine($"{p.Id}");
                Console.WriteLine($"\t{p.FirstName} {p.LastName}");
                Console.WriteLine($"\tPhone: {FormatPhone(p.Phone)}");
                Console.WriteLine($"\tCity: {p.City}");
                Console.WriteLine($"\tBalance: {p.Balance,20:C2}\n");
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
            auth.WriteLog("INFO", "Generar informe por ciudad", activeUser);
            Console.Clear();

            var peopleByCity = people
                .GroupBy(p => p.City)
                .OrderBy(g => g.Key);

            decimal total = 0;

            foreach (var group in peopleByCity)
            {
                Console.WriteLine($"Ciudad: {group.Key}\n");

                Console.WriteLine($"{"ID",-4} {"Nombres",-10} {"Apellidos",-12} {"Saldo",15}");
                Console.WriteLine($"{new string('-', 4)} {new string('-', 10)} {new string('-', 12)} {new string('-', 15)}");

                decimal subtotal = 0;

                foreach (var p in group)
                {
                    Console.WriteLine($"{p.Id,-4} {p.FirstName,-10} {p.LastName,-12} {p.Balance,15:N2}");
                    subtotal += p.Balance;
                }

                Console.WriteLine($"{new string(' ', 30)}{"=======".PadLeft(15)}");
                Console.WriteLine($"Total: {group.Key,-22} {subtotal,15:N2}\n");

                total += subtotal;
            }

            Console.WriteLine($"{new string(' ', 30)}{"=======".PadLeft(15)}");
            Console.WriteLine($"Total General:{total,26:N2}\n");
            break;

        case "0":
            helper.Write("people.csv", people);
            Console.WriteLine("Changes saved before exiting. Goodbye!");
            break;
    }
} while (option != "0");

string ShowMenu()
{
    Console.WriteLine("\n====== MAIN MENU ======");
    Console.WriteLine("1. View people\n2. Add\n3. Save\n4. Edit\n5. Delete\n6. Report\n0. Exit");
    Console.Write("Select: ");
    return Console.ReadLine() ?? "0";
}

string FormatPhone(string phone)
{
    var digits = new string(phone.Where(char.IsDigit).ToArray());
    if (digits.Length == 10)
        return $"{digits[..3]} {digits[3..6]} {digits[6..]}";
    else
        return phone;
}