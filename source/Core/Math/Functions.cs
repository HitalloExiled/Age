using System.Runtime.CompilerServices;
using SMath = System.Math;
using RealT = System.Single;
using System.Numerics;

namespace Age.Core.Math;

public static class Functions
{
    public const RealT CMP_EPSILON = 0.00001f;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsZeroApprox(RealT value) => RealT.Abs(value) < CMP_EPSILON;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T DegToRad<T>(T y) where T : notnull, INumber<T> => y * T.CreateChecked(SMath.PI / 180.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T RadToDeg<T>(T y) where T : notnull, INumber<T> => y * T.CreateChecked(180.0 / SMath.PI);
}
