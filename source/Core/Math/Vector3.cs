using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Age.Core.Math;

[DataContract]
internal record struct Vector3<T> where T : notnull, INumber<T>
{
    [DataMember]
    private readonly T[] axis = new T[3];

    [IgnoreDataMember]
    public T X
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.axis[0];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.axis[0] = value;
    }

    [IgnoreDataMember]
    public T Y
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.axis[1];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.axis[1] = value;
    }

    [IgnoreDataMember]
    public T Z
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.axis[2];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.axis[2] = value;
    }

    [IgnoreDataMember]
    public T this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.axis[i];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.axis[i] = value;
    }

    [IgnoreDataMember]
    public T LengthSquared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.Dot(this);
    }

    [IgnoreDataMember]
    public double Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => System.Math.Sqrt(double.CreateChecked(this.LengthSquared));
    }

    [IgnoreDataMember]
    public Vector3<T> Normalized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this / T.CreateSaturating(this.Length);
    }

    public Vector3() { }
    public Vector3(T x, T y, T z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T Dot(Vector3<T> other) =>
        this.X * other.X + this.Y * other.Y + this.Z * other.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Vector3<T> Cross(Vector3<T> vector) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator -(Vector3<T> value) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator *(Vector3<T> vector, T scalar) => throw new NotImplementedException();
    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator /(Vector3<T> left, T scalar) => throw new NotImplementedException();
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();
}
