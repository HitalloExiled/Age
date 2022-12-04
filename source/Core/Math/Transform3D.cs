using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Age.Core.Math;

// TODO - Analyze whether there is real gain when using generic types in core structures
[Serializable]
[DataContract]
internal record struct Transform3D<T> where T : notnull, INumber<T>, IRootFunctions<T>
{
    [DataMember]
    public Basis<T> Basis { get; set; }

    [DataMember]
    public Vector3<T> Origin { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Plane<T> XForm(Plane<T> plane) => Transform3D.XForm(this, plane);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Vector3<T> XForm(Vector3<T> vector) => Transform3D.XForm(this, vector);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Transform3D<T> Inverse() => Transform3D.Inverse(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Transform3D<T> operator *(Transform3D<T> left, Transform3D<T> right) => throw new NotImplementedException();
}

file static class Transform3D
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Plane<T> XForm<T>(Transform3D<T> transform, Plane<T> plane)
   where T : notnull, INumber<T>, IRootFunctions<T>
    {
        var basis = transform.Basis.Inverse().Transposed();

        return XFormFast(transform, plane, basis);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> XForm<T>(Transform3D<T> transform, Vector3<T> vector)
    where T : notnull, INumber<T>, IRootFunctions<T>
    {
        var origin = transform.Origin;
        var basis  = transform.Basis;
        return new(
            basis[0].Dot(vector) + origin.X,
            basis[1].Dot(vector) + origin.Y,
            basis[2].Dot(vector) + origin.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Transform3D<T> Inverse<T>(Transform3D<T> transform)
    where T : notnull, INumber<T>, IRootFunctions<T> =>
        transform with
        {
            Origin = transform.Basis.Transposed().XForm(-transform.Origin)
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static Plane<T> XFormFast<T>(Transform3D<T> transform, Plane<T> plane, Basis<T> basisInverseTranspose)
    where T : notnull, INumber<T>, IRootFunctions<T>
    {
        var point    = XForm(transform, plane.Normal * plane.Distance);
        var normal   = basisInverseTranspose.XForm(plane.Normal).Normalized;
        var distance = normal.Dot(point);

        return new Plane<T>(normal, distance);
    }
}

