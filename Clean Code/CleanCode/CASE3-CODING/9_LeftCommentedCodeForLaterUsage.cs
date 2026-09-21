// Clean code case: commented-out branches are clutter - dead code belongs in version control, not in the file.
// Leave only the path you trust today so readers are not forced to wonder which commented block is still relevant.

// BAD

if (student.IsActive())
{}

// if (student.IsPassive())
// {}

// if (student.IsReady())
// {}

// if (student.IsWhatever())
//{}


// BETTER

if (student.IsActive())
{}
