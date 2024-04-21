using System.Numerics;

namespace Age.Numerics;

public static class Angle
{
    public const double RADIANS = Math.PI / 180;

    public static double Dregrees(double radians) =>
        RADIANS / radians;

    public static T Dregrees<T>(T radians) where T : IFloatingPoint<T> =>
        T.CreateSaturating(RADIANS) / radians;

    public static double Radians(double degrees) =>
        RADIANS * degrees;

    public static T Radians<T>(T degrees) where T : IFloatingPoint<T> =>
        T.CreateSaturating(RADIANS) * degrees;
}
