// Clean code case: prefer positive, intention-revealing conditionals to spare readers from double negatives.
// Express the truth you want to check so the `if` reads like English instead of a logic puzzle.

// BAD

public bool IsNotPresent(T node)
{
    // ...
}

if (!IsNotPresent(node))
{
    // ...
}


// BETTER

public bool IsPresent(T node)
{
    // ...
}

if (IsNotPresent(node) == false)
{
    // ...
}


// EVEN BETTER

public bool IsPresent(T node)
{
    // ...
}

if (IsPresent(node))
{
    // ...
}

