// Clean code case: stick to consistent casing and naming conventions so symbols are predictable at a glance.
// When identifiers dance between ALL_CAPS, snake_case, and odd prefixes, your reader trips over style instead of logic.

// Bad

const int DAYS_IN_WEEKEND = 2;
const int activedaysinmonth = 30;

public DateTime Date_of_last_order;

public static class staticAnimal;
public class animal;


// Better
const int DaysInWeekend = 2;
const int ActiveDaysInMonth = 30;

public DateTime DateOfLastOrder;

public static class Animal;
public class Animal;
