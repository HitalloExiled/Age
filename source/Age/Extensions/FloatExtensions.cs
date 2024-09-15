namespace Age.Extensions;

public static class FloatExtensions
{
    public static float ClampSubtract(this float value, float other) =>
        value > other ? value - other : 0;
}
