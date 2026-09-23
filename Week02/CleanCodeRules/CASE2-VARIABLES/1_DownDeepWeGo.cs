// Clean code case: collapse unnecessary nesting so the happy path is obvious and guard clauses stop noise early.
// Deep indent ladders force readers to juggle conditions; flat, early returns let intent shine.

// Bad
public long FibonacciCore(int n)
{
    if (n < 50)
    {
        if (n != 0)
        {
            if (n != 1)
            {
                return FibonacciCore(n - 1) + FibonacciCore(n - 2);
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 0;
        }
    }
    else
    {
        throw new System.Exception("Not supported");
    }
}
        
// Better

public long Fibonacci(int n)
{
    if (n == 0)
    {
        return 0;
    }

    if (n == 1)
    {
        return 1;
    }

    if (n > 50)
    {
        throw new System.ArgumentException("N > 50 is not supported.");
    }

    return Fibonacci(n - 1) + Fibonacci(n - 2);
}
    
