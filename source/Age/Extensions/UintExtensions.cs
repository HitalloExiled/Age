namespace Age.Extensions;

public static class UintExtensions
{
    public static uint ClampSubtract(this uint value, uint other) =>
        value > other ? value - other : 0;
}
