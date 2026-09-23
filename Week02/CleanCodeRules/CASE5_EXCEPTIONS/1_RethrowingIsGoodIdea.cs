// Clean code case: rethrow exceptions without losing the original stack trace.
// Use plain `throw;` after logging so debugging points to the real fault line, not the catch block.

// BAD

try
{
    // WORK
}
catch (Exception exp)
{
    _logger.LogException("Ehhmm....");

    throw exp;
}

// BETTER

try
{
    // WORK
}
catch (Exception exp)
{
    _logger.LogException("Ehhmm....");
    throw;
}


