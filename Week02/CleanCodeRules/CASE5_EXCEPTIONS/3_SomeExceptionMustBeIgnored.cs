// Clean code case: never swallow exceptions silently - handle them meaningfully or propagate them.
// Ignored errors hide real failures; even a reconnect attempt or rethrow is better than pretending nothing happened.

// BAD

try
{
    // WORK
}
catch (HttpException) 
{

}
catch (Exception exp)
{
    throw;
}

// BETTER

try
{
    // WORK
}
catch (HttpException) 
{
    TryReconnect();
}
catch (Exception exp)
{
    //..
    throw;
}


