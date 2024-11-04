using System.Numerics;

namespace Age.Numerics;

public record struct AABB<T> where T : IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    public Vector3<T> Size;
    public Vector3<T> Position;

    public readonly T Volume => this.Size.X * this.Size.Y * this.Size.Z;

    public AABB(in Vector3<T> size, in Vector3<T> position)
    {
        this.Size     = size;
        this.Position = position;
    }

    public AABB(T width, T height, T depth, T x, T y, T z) : this(new(width, height, depth), new(x, y, z))
    { }

    public void Extends(in AABB<T> extends)
    {
        if (this == default)
        {
            this = extends;
        }
        else
        {
            var current = this;

            this.Position.X = T.Min(extends.Position.X, current.Position.X);
            this.Position.Y = T.Min(extends.Position.Y, current.Position.Y);
            this.Position.Z = T.Min(extends.Position.Z, current.Position.Z);

            this.Size.X = T.Max(extends.Position.X + extends.Size.X, current.Position.X + current.Size.X);
            this.Size.Y = T.Max(extends.Position.Y + extends.Size.Y, current.Position.Y + current.Size.Y);
            this.Size.Z = T.Max(extends.Position.Z + extends.Size.Z, current.Position.Z + current.Size.Z);

            this.Size -= this.Position;
        }
    }

    public void Extends(in Vector3<T> size, in Vector3<T> position) =>
        this.Extends(new(size, position));

    public readonly AABB<T> Intersection(in AABB<T> other)
    {
        var x = T.Max(this.Position.X, other.Position.X);
        var y = T.Max(this.Position.Y, other.Position.Y);
        var z = T.Max(this.Position.Z, other.Position.Z);

        var width  = T.Min(this.Size.X + this.Position.X, other.Size.X + other.Position.X) - x;
        var height = T.Min(this.Size.Y + this.Position.Y, other.Size.Y + other.Position.Y) - y;
        var depth  = T.Min(this.Size.Z + this.Position.Z, other.Size.Z + other.Position.Z) - z;

        return new(width, height, depth, x, y, z);
    }

    public readonly AABB<T> Merged(in AABB<T> other)
    {
        var aabb = this;

        aabb.Extends(other);

        return aabb;
    }

    public override readonly string ToString() =>
        $"{{ Size: {this.Size}, Position: {this.Position} }}";
}
