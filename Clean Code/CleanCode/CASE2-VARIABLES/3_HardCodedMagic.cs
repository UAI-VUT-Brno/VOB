// Clean code case: magic strings and numbers hide intent and invite typos - lift them into named constants.
// A well-named constant is self-documenting and keeps behavior aligned across the codebase.

// BAD
if (userRole == "Student")
{
    // TO DO
}

// BETTER
const string STUDENTROLE = "Student";

if (userRole == STUDENTROLE)
{
    // TO DO
}
