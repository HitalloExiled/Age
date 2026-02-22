using Age.Numerics;
using Age.Scenes;

namespace Age.Commands;

public abstract record Command2D : Command<Spatial<Command2D, Matrix3x2<float>>>
{
    public Matrix3x2<float> LocalMatrix { get; set; } = Matrix3x2<float>.Identity;
    public Size<uint>       Size        { get; set; }
    public int              ZIndex      { get; set; }

    public Matrix3x2<float> Matrix => this.LocalMatrix * this.Owner.Matrix;

    public Rect<float> GetAffineRect() => new(this.Size.Cast<float>(), this.LocalMatrix.Translation.ToPoint());

    public override void Reset()
    {
        base.Reset();

        this.LocalMatrix = Matrix3x2<float>.Identity;
        this.Size        = default;
        this.ZIndex      = default;
    }
}
