using Microsoft.VisualStudio.TestTools.UnitTesting;

// Clean code case: let MSTest describe intent with attributes and clear Arrange/Act/Assert flow.
// Avoid ad-hoc console tests; real tests should be isolated, repeatable, and self-checking.

// BAD

public class AdHocStudentTests
{
    public void Run()
    {
        var student = new Student { IsActive = false };
        var service = new StudentService();

        var result = service.CanEnroll(student);

        // TODO: remember to check result manually in console output...
        System.Console.WriteLine(result);
    }
}


// BETTER

[TestClass]
public class StudentServiceTests
{
    [TestMethod]
    public void CanEnroll_ReturnsTrue_ForActiveStudents()
    {
        // Arrange
        var service = new StudentService();
        var student = new Student { IsActive = true };

        // Act
        var canEnroll = service.CanEnroll(student);

        // Assert
        Assert.IsTrue(canEnroll, "Active students should be enrollable.");
    }
}

public class Student
{
    public bool IsActive { get; set; }
}

public class StudentService
{
    public bool CanEnroll(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        return student.IsActive;
    }
}
