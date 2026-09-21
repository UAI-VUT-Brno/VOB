// Clean code case: when a method needs a bundle of related fields, model the concept instead of spraying parameters.
// A dedicated `Student` object documents intent, keeps parameters in sync, and leaves room for future growth without overload chaos.

// BAD

public void RegisterStudent(string name, string fullName, DateTime birthDate, bool isErasmus)
{
    // ...
}


// BETTER

public class Student
{
    public string Name {get; init; }
    public string Name {get; init; }
    public DateTime BirthDate {get; init; }
    public bool IsErasmus {get; init; }
}

public void RegisterStudent(Student student) {}
