// Clean code case: method names should shout the context and outcome, not hide behind vague verbs like "Process".
// Make the call read like a sentence - "SendConfirmationEmail" tells teammates exactly what happens.

// BAD

public class Student
{
    public void Process()
    {
        SendMail(this.EmailAddress, new Email<Confirmation>("We confirm..."));
    }
}


// BETTER

public class Student
{
    public void SendConfirmationEmail()
    {
        SendMail(this.EmailAddress, new Email<Confirmation>("We confirm..."));
    }
}
