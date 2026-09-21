// Clean code case: use real structure markers (regions, methods) instead of decorative comment separators.
// Long lines of slashes and dashes add visual noise without improving navigation; lean on code organization instead.

// BAD

/// ------------------------------------------------
/// Helpers
/// ------------------------------------------------
if (student.IsActive())
{}

if (student.IsPassive())
{}

if (student.IsReady())
{}

if (student.IsWhatever())
{}

/// ------------------------------------------------
/// End of Helpers
/// ------------------------------------------------




// BETTER

#region Helpers

if (student.IsActive())
{}

if (student.IsPassive())
{}

if (student.IsReady())
{}

if (student.IsWhatever())
{}

#endregion
