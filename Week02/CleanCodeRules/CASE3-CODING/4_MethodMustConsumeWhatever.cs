// Clean code case: resist the urge to accept "anything" - stronger types document expectations and prevent surprises.
// Dynamic parameters force callers and maintainers to guess the real contract, while a typed signature keeps everyone honest.

// BAD

public double Calculate(dynamic value)
{
    int convertedValue;
    
    if (!int.TryParse(value, out convertedValue))
    {
        throw new Exception('Value must be of type double!');
    }

    return convertedValue * interest;
}


// BETTER

public double Calculate(double value)
{    
    return value * interest;
}
