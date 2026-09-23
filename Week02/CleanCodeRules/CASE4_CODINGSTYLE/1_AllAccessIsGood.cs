// Clean code case: keep fields private and expose behavior, not raw data, to guard invariants.
// If anyone can tweak credits directly, bugs creep in; controlled access enforces the rules in one place.

//BAD 

class Student
{
    public int Credits = 40;
}

var student = new Student();

student.Credits += 6;


// BETTER

class Student
{
    private int _credits = 40;

    public double Credits {
        get { return _credits; }
    }

    public Student(credits = 0)
    {
       _credits = credits;
    }

    public void AddCredits(int creditsToAdd)
    {
        if (creditsTodAdd <= 0)
        {
            throw new Exception('Credits must be positive integer bigger than 0!');
        }

        _credits += creditsToAdd;
    }
}
