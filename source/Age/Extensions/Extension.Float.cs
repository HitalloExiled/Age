namespace Age.Extensions;

public static partial class Extension
{
    public static float ClampSubtract(this float value, float other) =>
        value > other ? value - other : 0;
}
