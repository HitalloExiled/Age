using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Spatial3D : Renderable
{
    private CacheValue<Transform3D> transformCache;

    private Transform3D ParentTransform      => (this.Parent as Spatial3D)?.Transform ?? new();
    private Transform3D ParentTransformCache => (this.Parent as Spatial3D)?.TransformCache ?? new();
    private Transform3D PivotedTransform     => Transform3D.Translated(this.Pivot) * this.LocalTransform * Transform3D.Translated(-this.Pivot);

    internal static int CacheVersion { get; set; } = 1;

    internal virtual Transform3D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value = this.ParentTransformCache * this.PivotedTransform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    public virtual Transform3D LocalTransform
    {
        get;
        set => this.Set(ref field, value);
    } = new();

    public virtual Vector3<float> Pivot
    {
        get;
        set => this.Set(ref field, value);
    }

    public virtual Transform3D Transform
    {
        get => this.ParentTransform * this.PivotedTransform;
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }

    protected virtual void OnTransformChanged()
    { }

    protected void Set<T>(ref T field, in T value)
    {
        if (!Equals(field, value))
        {
            field = value;
            this.OnTransformChanged();
        }
    }
}
