// Clean code case: don't stutter with repetitive context in property names; classes already provide the backdrop.
// Trim the extra "User" prefixes so the important nouns pop instead of drowning in echoes.

// BAD

public class User
{
    public string UserName { get; set; }
    public string UserFullName { get; set; }
    public DateTime UserLastLogin { get; set; }
}


// BETTER

public class User
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public DateTime LastLogin { get; set; }
}
