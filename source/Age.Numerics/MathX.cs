using System.Numerics;

namespace Age.Numerics;

public static class MathX
{
    private const float EPSILON = 0.00001f;

    public static bool IsZeroApprox<T>(T value) where T : IFloatingPoint<T> =>
        value < T.CreateChecked(EPSILON);

    public static bool IsApprox<T>(T left, T right) where T : IFloatingPoint<T> =>
        T.Abs(left - right) < T.CreateChecked(EPSILON);

    public static T MinMax<T>(T min, T max, T value) where T : INumber<T> =>
        T.Min(max, T.Max(value, min));
}
