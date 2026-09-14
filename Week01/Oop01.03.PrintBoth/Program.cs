// VOB - Exercise 1, section 5.3
// TASK: why does only "Ahoj" get printed, and not "Novak"?
//       Fix it so that both are printed.

class Program
{
    static void Main()
    {
        Console.WriteLine("Ahoj");
    }
}

class SurnamePrinter
{
    public void Print()
    {
        Console.WriteLine("Novak");
    }
}
