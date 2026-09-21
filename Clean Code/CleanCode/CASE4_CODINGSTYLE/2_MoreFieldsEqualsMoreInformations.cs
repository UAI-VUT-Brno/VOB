// Clean code case: stuffing classes with fields does not equal clarity - carry only the data that represents the model.
// Extra IDs and unused properties are noise; model real relationships (like courses held by a student) instead of hoarding fields.

// BAD

class Student
{
    private Guid StudentID { get; private set; }

    private string Name { get; set; }
    private string Email { get; set; }

    public Employee(string name, string email)
    {
        Name = name;
        Email = email;
    }    
}

class Course 
{
    private Guid CourseID { get; private set; }    
    private Guid StudentID { get; private set; }    
    private string Name { get; set; }

    public Course()
    {
        //...
    }    
}


// BETTER
class Student
{
    private Guid StudentID { get; private set; }

    private string Name { get; set; }
    private string Email { get; set; }
    
    private List<Course> ActiveCourses = new();

    public Employee(string name, string email)
    {
        Name = name;
        Email = email;
    }    
}

class Course 
{
    private Guid CourseID { get; private set; }    
    private string Name { get; set; }

    public Course()
    {
        //...
    }    
}


