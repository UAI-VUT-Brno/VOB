// VOB - Exercise 1, section 5.2
// A class is a blueprint: fields are what the object knows,
// methods are what it can do. Static belongs to the class itself.

public class Motor
{
    public string Name;           // what the object knows
    float rpm = 0f;               // hidden from outside

    public void Start()           // what the object can do
    {
        rpm = 800f;
    }

    public float Increase(float by)    // hands a value back
    {
        rpm = rpm + by;
        return rpm;
    }

    public static string Describe()    // belongs to the class
    {
        return "A motor has a name and revolutions.";
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine(Motor.Describe());   // no object needed

        Motor m = new Motor();
        m.Name = "M1";
        m.Start();

        Console.WriteLine(m.Name + ": " + m.Increase(200));
    }
}
