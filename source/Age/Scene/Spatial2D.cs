using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Spatial2D : Renderable
{
    private protected CacheValue<Transform2D> TransformCache { get; set; }

    private Transform2D CachedParentTransform => (this.Parent as Spatial2D)?.CachedTransform ?? Transform2D.Identity;
    private Transform2D ParentTransform       => (this.Parent as Spatial2D)?.Transform ?? Transform2D.Identity;

    internal static int CacheVersion { get; set; } = 1;

    internal virtual Transform2D CachedTransform
    {
        get
        {
            if (this.TransformCache.Version != CacheVersion)
            {
                this.TransformCache = new()
                {
                    Value   = this.LocalTransform * this.CachedParentTransform,
                    Version = CacheVersion
                };
            }

            return this.TransformCache.Value;
        }
    }

    public virtual Transform2D LocalTransform { get; set; } = Transform2D.Identity;

    public virtual Transform2D Transform
    {
        get => this.LocalTransform * this.ParentTransform;
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }
}
