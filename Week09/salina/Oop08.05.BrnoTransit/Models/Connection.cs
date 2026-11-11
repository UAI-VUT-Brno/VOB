namespace BrnoTransit.Models
{
    // one direct ride between two neighbouring stops, as the timetable gives it
    // Minutes: average time of the ride; Trips: how many rides a week use it
    public record Connection(string From, string To, string Line, double Minutes, int Trips);
}
