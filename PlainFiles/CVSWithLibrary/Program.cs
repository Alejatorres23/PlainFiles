using CVSWithLibrary;

var helper = new CsvHelperExample();
var readPeople = helper.Read("people.csv").ToList();
var opc = "0";
do
{
    opc = Menu();
    Console.WriteLine("============================");
    switch (opc)
    {
        case "1":
            foreach (var person in readPeople)
            {
                Console.WriteLine(person);
            }
            break;

        case "2":
            Console.Write("Enter the ID: ");
            var id = Console.ReadLine();
            Console.Write("Enter the First name: ");
            var firstName = Console.ReadLine();
            Console.Write("Enter the Last name: ");
            var lastName = Console.ReadLine();
            Console.Write("Enter the phone: ");
            var phone = Console.ReadLine();
            Console.Write("Enter the city: ");
            var city = Console.ReadLine();
            Console.Write("Enter the balance: ");
            var balance = Console.ReadLine();

            var newperson = new Person
            {
                Id = int.Parse(id!),
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Phone = phone ?? string.Empty,
                City = city ?? string.Empty,
                Balance = decimal.Parse(balance!)
            };

            readPeople.Add(newperson);
            break;

        case "3":
            SaveChanges();
            break;
    }
} while (opc != "0");
SaveChanges();

void SaveChanges()
{
    helper.Write("people.csv", readPeople);
}

string Menu()
{
    Console.WriteLine("============================");
    Console.WriteLine("1. Show content");
    Console.WriteLine("2. Add a new person");
    Console.WriteLine("3. Save changes");
    Console.WriteLine("0. Exit");
    Console.Write("choose an option: ");
    return Console.ReadLine() ?? "0";
}