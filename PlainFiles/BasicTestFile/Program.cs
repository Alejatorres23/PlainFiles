using BasicTestFile;

var textFile = new SimpleTextFile("data.txt");
var Lines = textFile.ReadLines();

using (var logger = new LogWriter("log.text"))
{
    var opc = "0";
    logger.WriteLog("INFO", "Application started.");
    do
    {
        opc = Menu();
        Console.WriteLine("==========================");
        switch (opc)
        {
            case "1":
                logger.WriteLog("INFO", "Show content.");
                if (Lines.Length == 0)
                {
                    Console.WriteLine("Empty file.");
                    logger.WriteLog("ERROR", "Empty file.");
                    break;
                }
                foreach (var line in Lines)
                {
                    Console.WriteLine(line);
                }
                break;

            case "2":
                logger.WriteLog("INFO", "Add line.");
                Console.Write("Enter the line to add: ");
                var newLine = Console.ReadLine();
                if (!string.IsNullOrEmpty(newLine))
                {
                    Lines = Lines.Append(newLine).ToArray();
                }
                break;

            case "3":
                logger.WriteLog("INFO", "Update line.");
                Console.Write("Enter the line to remove: ");
                var lineToUpdate = Console.ReadLine();
                Console.Write("Enter the new value: ");
                var newValue = Console.ReadLine();
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i] == lineToUpdate)
                    {
                        Lines[i] = newValue!;
                        break;
                    }
                }
                break;

            case "4":
                logger.WriteLog("INFO", "Remove content.");
                Console.Write("Enter the line to remove: ");
                var lineToRemove = Console.ReadLine();
                if (!string.IsNullOrEmpty(lineToRemove))
                {
                    Lines = Lines.Where(line => line != lineToRemove).ToArray();
                }
                break;

            case "5":
                SaveChanges();
                logger.WriteLog("INFO", "Save file.");
                break;

            case "0":
                Console.WriteLine("Exiting...");
                break;
            default:
                Console.WriteLine("Invalid optio. Please try again.");
                break;

        }
    } while (opc != "0");
    logger.WriteLog("INFO", "Application ended.");
    SaveChanges();
}
void SaveChanges()
{
   Console.WriteLine("Saving changes...");
    textFile.WriteLines(Lines);
    Console.WriteLine("Changes saved.");
}

string Menu()
{
    Console.WriteLine("==========================");
    Console.WriteLine("1. Show content");
    Console.WriteLine("2. Add line");
    Console.WriteLine("3. Update line");
    Console.WriteLine("4. Remove line");
    Console.WriteLine("5. Save changes");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your option: ");
    return Console.ReadLine() ?? "0";
}