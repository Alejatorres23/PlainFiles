using CVSWithLibrary;

var list = new List<Person>
{
    new() { Id = 1, Name = "Maria", Age = 28 },
    new() { Id = 2, Name = "Juan", Age = 34 },
};
var helper = new CsvHelperExample();
helper.Write("people.csv", list);

var readPeople = helper.Read("people.csv");
foreach (var person in readPeople)
{
    Console.WriteLine($"Id: {person.Id}, Name: {person.Name}, Age: {person.Age}");
}