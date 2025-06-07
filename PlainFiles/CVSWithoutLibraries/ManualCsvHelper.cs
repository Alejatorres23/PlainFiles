namespace CVSWithoutLibraries;

public class ManualCsvHelper
{
    public void WriteCVS(string path, List<string[]> records)
    {
        using var sw = new StreamWriter(path);
        foreach (var record in records)
        {
            var line = string.Join(",", record ); 
            sw.WriteLine(line);
        }
    }

    public List<string[]> ReadCVS(string path)
    {
        var records = new List<string[]>();
        if (!File.Exists(path))
        {
            return records;
        }
        using var sr = new StreamReader(path);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var record = line.Split(',');
            records.Add(record);
        }
        
        return records;
    }
}
