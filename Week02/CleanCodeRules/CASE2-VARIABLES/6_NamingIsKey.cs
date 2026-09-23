// Clean code case: meaningful variable names beat cryptic abbreviations, especially when objects travel across layers.
// Describe the domain item ("student", "serializedStudent") so the data's shape and lifecycle are obvious.

// BAD

var d1 = new { Faculty = "FSI", StudentId = "1234567" };

var dStream = new MemoryStream();
WriteObject(dStream, DoParsingNow(d1));



// BETTER

var student = new Student 
    { 
        Faculty = "FSI", 
        StudentId = "1234567" 
    };

var memoryStream = new MemoryStream();
var serializedStudent = Serializer.Serialize<Student>(student);
//...
