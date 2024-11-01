using System.Numerics;

namespace Age.Numerics;

public static class Angle
{
    public static T RadiansToDegrees<T>(T radians) where T : IFloatingPoint<T> =>
        Math<T>.Degrees * radians;

    public static T DegreesToRadians<T>(T degrees) where T : IFloatingPoint<T> =>
        Math<T>.Radians * degrees;
}
