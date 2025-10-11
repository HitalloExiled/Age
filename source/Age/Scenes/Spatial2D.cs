using Age.Commands;
using Age.Core;
using Age.Numerics;

namespace Age.Scenes;

public abstract class Spatial2D : Renderable<Command2D>
{
    private protected CacheValue<Transform2D> TransformCache { get; set; }

    private protected Transform2D CachedParentTransform => (this.Parent as Spatial2D)?.CachedTransform ?? Transform2D.Identity;
    private protected Transform2D ParentTransform       => (this.Parent as Spatial2D)?.Transform ?? Transform2D.Identity;

    internal virtual Transform2D CachedTransform
    {
        get
        {
            if (this.TransformCache.IsInvalid)
            {
                this.TransformCache = new(this.LocalTransform * this.CachedParentTransform);
            }

            return this.TransformCache.Value;
        }
    }

    public virtual Transform2D LocalTransform { get; set; } = Transform2D.Identity;

    public new Scene2D? Scene
    {
        get => base.Scene as Scene2D;
        set => base.Scene = value;
    }

    public virtual Transform2D Transform
    {
        get => this.LocalTransform * this.ParentTransform;
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }
}
