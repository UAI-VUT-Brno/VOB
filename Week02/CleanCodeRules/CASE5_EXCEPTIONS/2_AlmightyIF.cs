// Clean code case: avoid one giant catch with branching - catch specific exceptions separately.
// Tailored catch blocks make the intent obvious and keep unrelated error handling from tangling together.

// BAD

try
{
    // WORK
}
catch (Exception exp)
{   
    if (exp.Message.Contain("Internet broken...")) 
    {

    }

    if (exp is HttpRequestException)
    {        
        // DO ...
    }
    else if (exp is Exception)
    {
        // It is really Exception
    }
}


// BETTER

try
{
    // WORK
}
catch (HttpRequestException) 
{
    TryReconnect();
}
catch (Exception exp)
{
    //..
    throw;
}


