using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Age.Core.Math;

[DataContract]
internal record struct Vector2<T> where T : notnull, INumber<T>
{
    [DataMember]
    private T x = T.Zero;

    [DataMember]
    private T y = T.Zero;

    public T X
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.x;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.x = value;
    }

    public T Y
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.y;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => this.y = value;
    }

    public T this[int index]
    {
        get
        {
            if (index == 0)
            {
                return this.x;
            }
            else if (index == 1)
            {
                return this.y;
            }

            throw new IndexOutOfRangeException();
        }
    }

    public Vector2() { }

    public Vector2(T x, T y) : this()
    {
        this.X = x;
        this.Y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Vector2<TOther> As<TOther>() where TOther : notnull, INumber<TOther> =>
        new (TOther.CreateSaturating(this.X), TOther.CreateSaturating(this.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator +(Vector2<T> vector) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator -(Vector2<T> vector) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator *(Vector2<T> vector, T scalar) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator /(Vector2<T> vector, T scalar) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right) => throw new NotImplementedException();
}
