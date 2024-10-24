using System.Numerics;

namespace Age.Numerics;

public static class Angle
{
    public const double RADIANS = Math.PI / 180;
    public const double DEGREES = 180 / Math.PI;

    public static double Degrees(double radians) =>
        radians * DEGREES;

    public static float Degrees(float radians) =>
        radians * (float)DEGREES;

    public static T Degrees<T>(T radians) where T : IFloatingPoint<T> =>
        radians * T.CreateSaturating(DEGREES);

    public static double Radians(double degrees) =>
        degrees * RADIANS;

    public static float Radians(float degrees) =>
        degrees * (float)RADIANS;

    public static T Radians<T>(T degrees) where T : IFloatingPoint<T> =>
        T.CreateSaturating(RADIANS) * degrees;
}
