// Clean code case: code should be readable without relying on the reader's short-term memory or variable decoding skills.
// Name things after what they represent and iterate clearly so nobody has to keep a mental cheat sheet.

// Bad
var u = new[] { "FSI", "FEKT", "FIT" };

for (var i = 0; i < l.Count(); i++)
{
    var it = u[i];
    
    LogToSome();
    ReadValue();

    // ...
    // ...
    // ...

    Remove(it);
}
        
        
// Better

var faculties = new[] { "FSI", "FEKT", "FIT" };

foreach (var faculty in faculties)
{
    LogToSome();
    ReadValue();

    // ...
    // ...
    // ...

    Remove(faculty);
}
