using System;
using System.Threading.Tasks;

public class Program
{
    public static void Main()
    {
        Task<int> task = new Task<int>(ComputeValue);

        task.Start();

        int result = task.Result;

        Console.WriteLine($"Task result: {result}");
    }

    static int ComputeValue()
    {
        int sum = 0;

        for (int i = 1; i <= 10; i++)
        {
            sum += i;
        }

        return sum;
    }
}