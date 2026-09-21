// Clean code case: property names should tell the real story, not hide behind single letters.
// Future readers should instantly grasp dates like "Creation" and "LastModification" without guessing what "o" or "l" mean.

// Bad

public class Order
{
    public Datetime oDate { get; set; }
    public Datetime lDate { get; set; }
}


// Better
public class Order
{
    public Datetime CreationDate { get; set; }
    public Datetime LastModificationTime { get; set; }
}
