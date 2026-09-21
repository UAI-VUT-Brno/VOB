// Clean code case: avoid mega-methods that fetch, validate, dismiss, and email in one breath - split responsibilities.
// Small focused functions are easier to test, reuse, and reason about than one "do it all" pipeline.

// BAD

using System.Threading.Tasks;

public async Task FindStudentDismissAndSendEmail(List<string> students)
{
    foreach (var student in students)
    {
        Student? dbStudent = _studentService.GetByName(student);

        if(dbStudent is null)
        {
            throw new System.Exception("Student not found!");
        }

        try
        {
            await DismissStudent(dbStudent);
        }
        catch (StudnetAlreadyDismissedException)
        {
            throw;
        }

        if (string.NullOrEmpty(dbStudent.Email))
        {
            await SendEmail(client);
        }
        //...
    }
}

// BETTER

public async Task SendEmail(Student student)
{
    if (string.NullOrEmpty(dbStudent.Email))
    {
        throw new System.Exception("Student has no email address");
    }
    
    await Client.SendEmail(dbStudent.Email);
}

public Student Search(string studentName)
{
    return _studentService.GetStudentByName();
}

public Student Search(Student student)
{
    return _studentService.GetStudent(student.Id);
}
