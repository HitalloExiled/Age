using System.Numerics;
using System.Runtime.InteropServices;
using Age.Numerics;

namespace Age.Styling;

[StructLayout(LayoutKind.Explicit)]
public partial struct TransformOp
{
    [FieldOffset(0)]
    internal TransformOpKind Kind;

    [FieldOffset(4)]
    internal TransformOpData Data;

    internal static Transform2D Resolve(in TransformOp transform, in Size<uint> size, uint fontSize)
    {
        if (transform.Kind == TransformOpKind.Translation)
        {
            var x = Unit.Resolve(transform.Data.Translation.X, size.Width,  fontSize);
            var y = Unit.Resolve(transform.Data.Translation.Y, size.Height, fontSize);

            return Transform2D.CreateTranslated(x, -y);
        }

        return transform.Kind switch
        {
            TransformOpKind.Rotation => Transform2D.CreateRotated(transform.Data.Rotation),
            TransformOpKind.Scale    => Transform2D.CreateScaled(transform.Data.Scale.ToVector2()),
            TransformOpKind.Matrix   => new(transform.Data.Matrix),
            _ => throw new NotImplementedException(),
        };
    }

    public static TransformOp Translate(PointUnit translation) =>
        new()
        {
            Kind  = TransformOpKind.Translation,
            Data = new() { Translation = translation },
        };

    public static TransformOp Translate(Unit x, Unit y) =>
        Translate(new(x, y));

    public static TransformOp Rotate(float rotation) =>
        new()
        {
            Kind  = TransformOpKind.Rotation,
            Data = new() { Rotation = rotation },
        };

    public static TransformOp Scale(Point<float> scale) =>
        new()
        {
            Kind  = TransformOpKind.Scale,
            Data = new() { Scale = scale },
        };

    public static TransformOp Scale(float scaleX, float scaleY) =>
        Scale(new Point<float>(scaleX, scaleY));

    public static TransformOp Scale(float scale) =>
        Scale(new Point<float>(scale, scale));

    public static TransformOp Skew(Point<float> skew) =>
        new()
        {
            Kind = TransformOpKind.Skew,
            Data = new() { Skew = skew },
        };

    public static TransformOp Skew(float skewX, float skewY) =>
        Skew(skewX, skewY);

    public static TransformOp Matrix(Matrix3x2<float> matrix) =>
        new()
        {
            Kind  = TransformOpKind.Matrix,
            Data = new() { Matrix = matrix },
        };

    public static TransformOp Matrix(float m11, float m12, float m21, float m22, float m31, float m32) =>
        new()
        {
            Kind  = TransformOpKind.Matrix,
            Data = new() { Matrix = new(m11, -m12, -m21, m22, m31, -m32) },
        };

    public static TransformOp Matrix(in Vector2<float> column1, in Vector2<float> column2, in Vector2<float> column3) =>
        Matrix(column1.X, column1.Y, column2.X, column2.Y, column3.X, column3.Y);

    public override readonly string ToString() =>
        this.Kind switch
        {
            TransformOpKind.Translation => $"Translation: {{ {this.Data.Translation} }}",
            TransformOpKind.Rotation    => $"Rotation:    {{ {this.Data.Rotation}rad }}",
            TransformOpKind.Scale       => $"Scale:       {{ {this.Data.Scale} }}",
            TransformOpKind.Skew        => $"Skew:        {{ {this.Data.Skew} }}",
            TransformOpKind.Matrix      => $"Matrix:      {{ {this.Data.Matrix} }}",
            _ => throw new NotSupportedException(),
        };
}
