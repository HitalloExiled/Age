using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Spatial2D : Renderable
{
    internal static int CacheVersion { get; set; } = 1;

    private CacheValue<Transform2D> transformCache;

    private Transform2D ParentTransform      => (this.Parent as Spatial2D)?.Transform ?? Transform2D.Identity;
    private Transform2D ParentTransformCache => (this.Parent as Spatial2D)?.TransformCache ?? Transform2D.Identity;

    internal virtual Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value   = this.ParentTransformCache * this.LocalTransform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    public virtual Transform2D LocalTransform
    {
        get;
        set => this.Set(ref field, value, this.TransformChanged);
    } = Transform2D.Identity;

    public virtual Transform2D Transform
    {
        get => this.ParentTransform * this.LocalTransform;
        set => this.LocalTransform = this.ParentTransform.Inverse() * value * this.LocalTransform.Inverse();
    }

    protected void Set<T>(ref T field, in T value, Action callback) where T : IEquatable<T>
    {
        if (!field.Equals(value))
        {
            field = value;

            callback.Invoke();

            if (this.Tree is RenderTree renderTree)
            {
                renderTree.MakeDirty();
            }
        }
    }

    protected virtual void TransformChanged()
    { }
}
