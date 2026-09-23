// Clean code case: prefer immutable design so objects cannot drift after creation.
// Locked-down state makes reasoning and testing easier, and pushes changes through explicit, controlled paths.

// BAD

class Student
{
    public string Name { get; set; }
    public List<string> Courses { get; set; } = new();
}

var student = new Student();
student.Name = "Alice";
student.Courses.Add("Math 101");
student.Name = "Bob"; // State silently changes mid-flight.


// BETTER

class ImmutableStudent
{
    public string Name { get; }
    public IReadOnlyCollection<string> Courses { get; }

    public ImmutableStudent(string name, IEnumerable<string> courses)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Courses = courses?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(courses));
    }

    public ImmutableStudent Enroll(string course)
    {
        if (string.IsNullOrWhiteSpace(course))
        {
            throw new ArgumentException("Course is required.", nameof(course));
        }

        return new ImmutableStudent(
            Name,
            Courses.Concat(new[] { course })
        );
    }
}

var immutableStudent = new ImmutableStudent("Alice", new[] { "Math 101" });
var updatedStudent = immutableStudent.Enroll("Physics 201"); // Original instance stays untouched.
