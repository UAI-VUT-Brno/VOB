using Microsoft.VisualStudio.TestTools.UnitTesting;

// Clean code case: keep each test focused on one behavior so failures point to a single cause.
// Mixing unrelated assertions makes diagnosis painful; split scenarios into separate tests.

[TestClass]
public class StudentServiceBehaviorTests
{
    // BAD: enroll logic and welcome email are coupled in one test - one failure obscures the other.
    [TestMethod]
    public void CanEnrollAndSendsEmail_AllInOne()
    {
        var service = new StudentService();
        var student = new Student { IsActive = true };

        var canEnroll = service.CanEnroll(student);
        Assert.IsTrue(canEnroll);

        var emailSent = service.SendWelcomeEmail(student);
        Assert.IsTrue(emailSent);
    }

    // BETTER: each behavior is proven independently.
    [TestMethod]
    public void CanEnroll_ReturnsTrue_ForActiveStudents()
    {
        var service = new StudentService();
        var student = new Student { IsActive = true };

        var canEnroll = service.CanEnroll(student);

        Assert.IsTrue(canEnroll);
    }

    [TestMethod]
    public void SendWelcomeEmail_ReturnsTrue_ForActiveStudents()
    {
        var service = new StudentService();
        var student = new Student { IsActive = true };

        var emailSent = service.SendWelcomeEmail(student);

        Assert.IsTrue(emailSent);
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

    public bool SendWelcomeEmail(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        if (!student.IsActive) return false;
        // Pretend we sent an email.
        return true;
    }
}
