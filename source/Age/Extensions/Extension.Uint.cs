namespace Age.Extensions;

public static partial class Extension
{
    public static uint ClampSubtract(this uint value, uint other) =>
        value > other ? value - other : 0;
}
