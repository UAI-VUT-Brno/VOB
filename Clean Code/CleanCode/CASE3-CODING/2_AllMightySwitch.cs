// Clean code case: giant type-switches scream for polymorphism - let each animal own its leg-count logic.
// When behavior hangs off type strings, adding a new animal breaks the switch; interfaces keep it open for extension.

// BAD

class Animal
{
    // ...

    public int GetNumberOfLegs()
    {
        switch (_type.ToString())
        {
            case 'Dog':
                return 4;
            case 'Cat':
                return 4;
            case 'Bird':
                return 2;
        }
    }
}

// BETTER

interface IAnimal
{
    // ...

    int GetNumberOfLegs();
}

class Dog : IAnimal
{
    // ...

    public int GetNumberOfLegs()
    {
        return 4;
    }
}

class Bird : IAnimal
{
    // ...

    public int GetNumberOfLegs()
    {
        return 2;
    }
}
