using System.Numerics;

namespace Age.Numerics;

public static class MathX
{
    public static bool IsZeroApprox<T>(T value) where T : IFloatingPoint<T> =>
        value < T.CreateChecked(0.000001);

    public static bool IsApprox<T>(T left, T right) where T : IFloatingPoint<T> =>
        T.Abs(left - right) < T.CreateChecked(0.000001);
}
