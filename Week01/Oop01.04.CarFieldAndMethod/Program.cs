// VOB - Exercise 1, section 6.1
// A class with a field and a method, and one object built from it.
// TASK: add a field Year and a method SlowDown(float by).
//       The speed must never drop below zero.

public class Car
{
    public string Brand;           // field: state of the object
    public float Speed;

    public void SpeedUp(float by)  // method: behaviour
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
        Car myCar = new Car();     // the object is born here
        myCar.Brand = "Skoda";
        myCar.Speed = 0;

        myCar.SpeedUp(50);
        myCar.Print();             // Skoda: 50 km/h
    }
}
