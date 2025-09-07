using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Spatial2D : Renderable
{
    private CacheValue<Transform2D> transformCache { get; set; }

    private Transform2D CachedParentTransform => (this.Parent as Spatial2D)?.CachedTransform ?? Transform2D.Identity;
    private Transform2D ParentTransform       => (this.Parent as Spatial2D)?.Transform ?? Transform2D.Identity;

    internal virtual Transform2D CachedTransform
    {
        get
        {
            if (this.transformCache.IsInvalid)
            {
                this.transformCache = new(this.LocalTransform * this.CachedParentTransform);
            }

            return this.transformCache.Value;
        }
    }

    public virtual Transform2D LocalTransform { get; set; } = Transform2D.Identity;

    public virtual Transform2D Transform
    {
        get => this.LocalTransform * this.ParentTransform;
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }
}
