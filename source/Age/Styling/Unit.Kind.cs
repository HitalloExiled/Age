namespace Age.Styling;

public partial record struct Unit
{
    internal enum Kind
    {
        Pixel      = 1,
        Percentage = 2,
        Em         = 3,
    }
}
