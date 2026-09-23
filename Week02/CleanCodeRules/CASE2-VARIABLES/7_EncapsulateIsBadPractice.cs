// Clean code case: push state checks behind clear methods so callers don't poke at raw values.
// Wrapping `IsActive` inside the object keeps the rule in one spot and reads like real intent at call sites.

// BAD

if (student.Status == "Active")
{
}


// BETTER

if (student.IsActive())
{}
