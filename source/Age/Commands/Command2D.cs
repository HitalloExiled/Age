using Age.Numerics;

namespace Age.Commands;

public abstract record Command2D : Command
{
    internal Transform2D Transform { get; set; } = Transform2D.Identity;

    public Transform2D LocalTransform { get; set; } = Transform2D.Identity;
    public Size<float> Size           { get; set; }
    public int         ZIndex         { get; set; }

    public Rect<float> GetAffineRect() => new(this.Size, this.LocalTransform.Position.ToPoint());

    public override void Reset()
    {
        base.Reset();

        this.Transform      = Transform2D.Identity;
        this.LocalTransform = Transform2D.Identity;
        this.Size           = default;
        this.ZIndex         = default;
    }
}
