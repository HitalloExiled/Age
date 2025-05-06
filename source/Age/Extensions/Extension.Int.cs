namespace Age.Extensions;

public static partial class Extension
{
    public static int ClampSubtract(this int value, int other) =>
        value > other ? value - other : 0;
}
