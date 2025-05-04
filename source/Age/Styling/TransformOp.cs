using System.Runtime.InteropServices;
using Age.Numerics;

namespace Age.Styling;

[StructLayout(LayoutKind.Explicit)]
public partial struct TransformOp
{
    [FieldOffset(0)]
    internal TransformOpKind Kind;

    [FieldOffset(4)]
    internal TransformOpValue Value;

    internal static Transform2D Resolve(in TransformOp transform, in Size<uint> size, uint fontSize)
    {
        if (transform.Kind == TransformOpKind.Translation)
        {
            var x = Unit.Resolve(transform.Value.Translation.X, size.Width,  fontSize);
            var y = Unit.Resolve(transform.Value.Translation.Y, size.Height, fontSize);

            return Transform2D.CreateTranslated(x, y);
        }

        return transform.Kind switch
        {
            TransformOpKind.Rotation => Transform2D.CreateRotated(transform.Value.Rotation),
            TransformOpKind.Scale    => Transform2D.CreateScaled(transform.Value.Scale.ToVector2()),
            TransformOpKind.Matrix   => new(transform.Value.Matrix),
            _ => throw new NotImplementedException(),
        };
    }

    public static TransformOp Translate(PointUnit translation) =>
        new()
        {
            Kind  = TransformOpKind.Translation,
            Value = new() { Translation = translation },
        };

    public static TransformOp Translate(Unit x, Unit y) =>
        Translate(new(x, y));

    public static TransformOp Rotate(float rotation) =>
        new()
        {
            Kind  = TransformOpKind.Rotation,
            Value = new() { Rotation = rotation },
        };

    public static TransformOp Scale(Point<float> scale) =>
        new()
        {
            Kind  = TransformOpKind.Scale,
            Value = new() { Scale = scale },
        };

    public static TransformOp Scale(float scaleX, float scaleY) =>
        Scale(scaleX, scaleY);

    public static TransformOp Scale(float scale) =>
        Scale(scale, scale);

    public static TransformOp Skew(Point<float> skew) =>
        new()
        {
            Kind = TransformOpKind.Skew,
            Value = new() { Skew = skew },
        };

    public static TransformOp Skew(float skewX, float skewY) =>
        Skew(skewX, skewY);

    public static TransformOp Matrix(Matrix3x2<float> matrix) =>
        new()
        {
            Kind  = TransformOpKind.Matrix,
            Value = new() { Matrix = matrix },
        };

    public override readonly string ToString() =>
        this.Kind switch
        {
            TransformOpKind.Translation => $"Translation: {{ {this.Value.Translation} }}",
            TransformOpKind.Rotation    => $"Rotation:    {{ {this.Value.Rotation}rad }}",
            TransformOpKind.Scale       => $"Scale:       {{ {this.Value.Scale} }}",
            TransformOpKind.Skew        => $"Skew:        {{ {this.Value.Skew} }}",
            TransformOpKind.Matrix      => $"Matrix:      {{ {this.Value.Matrix} }}",
            _ => throw new NotSupportedException(),
        };
}
