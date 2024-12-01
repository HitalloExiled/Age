using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Node2D : RenderNode
{
    internal static int CacheVersion { get; set; } = 1;

    private CacheValue<Transform2D> transformCache;

    private Transform2D ParentTransform      => (this.Parent as Node2D)?.Transform ?? new();
    private Transform2D ParentTransformCache => (this.Parent as Node2D)?.TransformCache ?? new();

    internal protected virtual Transform2D TransformCache
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
    } = new();

    public virtual Transform2D Transform
    {
        get => this.ParentTransform * this.LocalTransform;
        set => this.LocalTransform = this.ParentTransform.Inverse() * value * this.LocalTransform.Inverse();
    }

    protected void Set<T>(ref T field, in T value, Action callback)
    {
        if (!Equals(field, value))
        {
            field = value;

            callback.Invoke();

            if (this.Tree is RenderTree renderTree)
            {
                renderTree.IsDirty = true;
            }
        }
    }

    protected virtual void TransformChanged()
    { }
}
