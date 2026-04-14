namespace Plant.Core;

public class SafetyInterlock
{
    // a flag representing whether safety mode is active
    private bool _armed;

    // TASK:
    // Create a void public method RunGuarded that takes Action delegate as a parameter):
    //
    // This method should simulate running a machine operation
    // under a safety interlock controlled by this class.
    //
    // Behaviour requirements:
    // - print to console that the safety interlock is being activated
    // - internally switch the _armed flag to "active"
    // - !! execute the provided action (this is the callback) !!
    // - after the action finishes, print that the interlock is being released
    // - switch the _armed flag back to inactive
    //
    // Important:
    // The caller should NOT manage the sequence of these steps.
    // The purpose of this method is to guarantee the correct order.
}