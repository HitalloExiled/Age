using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements;

public abstract class ContainerNode : Node2D
{
    internal abstract Layout Layout { get; }

    private CacheValue<Transform2D> transformCache;

    internal protected override Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value   = this.Layout.Transform * base.TransformCache,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    public override Transform2D Transform
    {
        get => this.Layout.Transform * base.Transform;
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public Element? ParentElement => this.Parent as Element;

    public Rect<int> GetBoundings()
    {
        this.Layout.Update();

        var size     = this.Layout.Boundings.Cast<int>();
        var position = new Point<int>((int)this.Transform.Position.X, (int)this.Transform.Position.Y);

        return new(size, position);
    }
}
