namespace Plant.Services;

public class StationOperations
{
    // name of the station where the operation happens, includes various
    // possible operations
    private readonly string _name;

    public StationOperations(string name)
    {
        _name = name;
    }

    // TASK:
    // Create a method with no parameters called PulseValve().
    // It should simulate a short valve pulse.
    // The method should print a message to the console including
    // the station name and information that the valve was opened briefly.

    // TASK:
    // Create another method with no parameters called JogAxis().
    // It should simulate a small movement of a machine axis.
    // The method should print a message describing the action
    // and include the station name.
}