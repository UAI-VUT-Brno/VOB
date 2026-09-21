// Clean code case: avoid piling synonyms around the same concept - pick one clear verb and stick with it.
// When every method sounds alike, callers cannot tell which one matters; a single, well-named entry point removes doubt.

// BAD

GetStudentInfo();
GetStudentData();
GetStudentRecord();
GetStudentProfile();

// BETTER

GetStudent();
