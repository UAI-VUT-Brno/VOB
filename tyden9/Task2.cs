Task2 task = new Task2();
task.Run();

public class Task2
{
    public void Run()
    {
        var parts = new List<MachinePart>
        {
            new MachinePart { Id = 1, Name = "Shaft", Weight = 12.5, IsInStock = true },
            new MachinePart { Id = 2, Name = "Bolt", Weight = 0.3, IsInStock = true },
            new MachinePart { Id = 3, Name = "Housing", Weight = 18.2, IsInStock = false },
            new MachinePart { Id = 4, Name = "Gear", Weight = 6.8, IsInStock = true },
            new MachinePart { Id = 5, Name = "Bearing", Weight = 1.2, IsInStock = true }
        };

        PrintHeader();

        var heavyParts = GetHeavyParts(GetAvailableParts(parts));
        var reportLines = FormatReport(heavyParts);

        PrintReport(reportLines);
    }

    void PrintHeader()
    {
        Console.WriteLine("Machine parts report");
        Console.WriteLine("--------------------");
    }

    List<MachinePart> GetAvailableParts(List<MachinePart> parts)
    {
        var result = new List<MachinePart>();

        foreach (var part in parts)
        {
            if (part.IsInStock)
            {
                result.Add(part);
            }
        }

        return result;
    }

    List<MachinePart> GetHeavyParts(List<MachinePart> parts)
    {
        var result = new List<MachinePart>();

        foreach (var part in parts)
        {
            if (part.Weight > 5.0)
            {
                result.Add(part);
            }
        }

        return result;
    }

    List<string> FormatReport(List<MachinePart> parts)
    {
        var result = new List<string>();

        foreach (var part in parts)
        {
            result.Add($"{part.Name} - {part.Weight} kg");
        }

        return result;
    }

    void PrintReport(List<string> lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}

public class MachinePart
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Weight { get; set; }
    public bool IsInStock { get; set; }
}