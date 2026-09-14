// VOB - Exercise 1, section 6.3
// A method that returns a value, and a method that takes another object.
// TASK: add Describe() returning a string, and a method deciding whether
//       the package is small or large (limit 1000 cm3).
// TASK: read the three dimensions from the console instead of the code.

public class Package
{
    public float Width;
    public float Height;
    public float Depth;

    public float Volume()                    // returns a value
    {
        return Width * Height * Depth;
    }

    public bool IsBiggerThan(Package other)  // takes an object
    {
        return Volume() > other.Volume();
    }
}

class Program
{
    static void Main()
    {
        Package small = new Package();
        small.Width = 10; small.Height = 10; small.Depth = 5;

        Package medium = new Package();
        medium.Width = 20; medium.Height = 20; medium.Depth = 10;

        Package large = new Package();
        large.Width = 40; large.Height = 30; large.Depth = 25;

        Console.WriteLine(small.Volume());
        Console.WriteLine(large.IsBiggerThan(medium));
    }
}
