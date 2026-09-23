// Clean code case: checking runtime types and branching is brittle; lean on polymorphism to ask objects to act.
// The moment you add another animal, the cascading if blocks grow - push behavior into the type instead.

// BAD

public Food PreferedFood(object animal)
{
    if (animal.GetType() == typeof(Bird))
    {
        (animal as Bird).GetPreferedFood();
    }
    else if (animal.GetType() == typeof(Dog))
    {
        (animal as Dog).GetPreferedFood();
    }
}

// BETTER

public Food PreferedFood(Animal animal)
{
    animal.GetPreferedFood();
}
