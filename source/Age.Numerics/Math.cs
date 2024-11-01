using System.Numerics;

namespace Age.Numerics;

public static class Math<T> where T : INumber<T>
{
    public static T Degrees { get; } = T.CreateChecked(180 / Math.PI);
    public static T Epsilon { get; } = T.CreateChecked(0.00001f);
    public static T Half    { get; } = T.CreateChecked(0.5);
    public static T PI      { get; } = T.CreateChecked(Math.PI);
    public static T Radians { get; } = T.CreateChecked(Math.PI / 180);
    public static T Tau     { get; } = T.CreateChecked(Math.Tau);
    public static T Two     { get; } = T.One + T.One;

    public static bool IsZeroApprox(T value) =>
        value < Epsilon;

    public static bool IsApprox(T left, T right) =>
        T.Abs(left - right) < Epsilon;

    public static T MinMax(T min, T max, T value) =>
        T.Min(max, T.Max(value, min));
}
