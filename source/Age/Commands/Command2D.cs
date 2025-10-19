using Age.Numerics;
using Age.Scenes;

namespace Age.Commands;

public abstract record Command2D : Command<Spatial2D>
{
    public Transform2D LocalTransform { get; set; } = Transform2D.Identity;
    public Size<uint>  Size           { get; set; }
    public int         ZIndex         { get; set; }

    public Transform2D Transform => this.LocalTransform * this.Owner.CachedTransform;

    public Rect<float> GetAffineRect() => new(this.Size.Cast<float>(), this.LocalTransform.Position.ToPoint());

    public override void Reset()
    {
        base.Reset();

        this.LocalTransform = Transform2D.Identity;
        this.Size           = default;
        this.ZIndex         = default;
    }
}
