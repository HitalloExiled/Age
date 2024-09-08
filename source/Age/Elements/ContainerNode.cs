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
                    Value   = base.TransformCache * this.Layout.Transform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    public override Transform2D Transform
    {
        get => base.Transform * this.Layout.Transform;
        set => this.LocalTransform = base.Transform.Inverse() * value * this.Layout.Transform.Inverse();
    }
}
