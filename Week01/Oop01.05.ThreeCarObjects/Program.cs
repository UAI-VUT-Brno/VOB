// VOB - Exercise 1, section 6.2
// One class, three objects - each of them carries its own values.
// TASK: only one of the three cars ended up faster than it started.
//       Which one, and why? What would a static field do instead?
// TASK: put the three cars into a list and speed all of them up in a loop.

public class Car
{
    public string Brand;
    public float Speed;

    public void SpeedUp(float by)
    {
        Speed = Speed + by;
    }

    public void Print()
    {
        Console.WriteLine(Brand + ": " + Speed + " km/h");
    }
}

class Program
{
    static void Main()
    {
        Car a = new Car();  a.Brand = "Skoda"; a.Speed = 0;
        Car b = new Car();  b.Brand = "Tatra"; b.Speed = 20;
        Car c = new Car();  c.Brand = "Praga"; c.Speed = 60;

        a.SpeedUp(30);
        b.SpeedUp(10);

        a.Print();
        b.Print();
        c.Print();
    }
}
