using Microsoft.VisualStudio.TestTools.UnitTesting;

// Clean code case: test names should read like a specification, not a mystery placeholder.
// Descriptive names make failures self-explanatory and save everyone from opening the test to guess what broke.

[TestClass]
public class TestNamingExamples
{
    // BAD: the name hides the scenario and expected outcome.
    [TestMethod]
    public void Test1()
    {
        var service = new StudentService();
        var student = new Student { IsActive = true };

        var canEnroll = service.CanEnroll(student);

        Assert.IsTrue(canEnroll);
    }

    // BETTER: the name tells the story - given an active student, we expect enrollment to succeed.
    [TestMethod]
    public void CanEnroll_ReturnsTrue_WhenStudentIsActive()
    {
        var service = new StudentService();
        var student = new Student { IsActive = true };

        var canEnroll = service.CanEnroll(student);

        Assert.IsTrue(canEnroll);
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
