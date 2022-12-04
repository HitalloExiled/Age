using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Age.Core.Math;

[DataContract]
internal record struct Vector4<T> where T : notnull, INumber<T>, IRootFunctions<T>
{
    [DataMember]
    private readonly T[] axis = new T[4];

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
    public T W
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.axis[3];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.axis[3] = value;
    }

    [IgnoreDataMember]
    public T this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.axis[i];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.axis[i] = value;
    }

    public Vector4() { }

    public Vector4(T x, T y, T z, T w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    public Vector4(Vector3<T> vector, T w) : this(vector.X, vector.Y, vector.Z, w) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T Dot(Vector4<T> other) =>
        this.X * other.X + this.Y * other.X + this.Z * other.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator +(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator -(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator -(Vector4<T> value) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator *(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator /(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();
}
