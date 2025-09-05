using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Spatial3D : Renderable
{
    private protected CacheValue<Transform3D> TransformCache { get; set; }

    private Transform3D CachedParentTransform => (this.Parent as Spatial3D)?.CachedTransform ?? Transform3D.Identity;
    private Transform3D ParentTransform       => (this.Parent as Spatial3D)?.Transform ?? Transform3D.Identity;
    private Transform3D PivotedTransform      => Transform3D.Translated(this.Pivot) * this.LocalTransform * Transform3D.Translated(-this.Pivot);

    internal static int CacheVersion { get; set; } = 1;

    internal virtual Transform3D CachedTransform
    {
        get
        {
            if (this.TransformCache.Version != CacheVersion)
            {
                this.TransformCache = new()
                {
                    Value   = this.PivotedTransform * this.CachedParentTransform,
                    Version = CacheVersion
                };
            }

            return this.TransformCache.Value;
        }
    }

    public virtual Transform3D LocalTransform { get; set; } = Transform3D.Identity;

    public virtual Vector3<float> Pivot { get; set; }

    public virtual Transform3D Transform
    {
        get => this.PivotedTransform * this.LocalTransform * this.ParentTransform;
        set => this.LocalTransform = this.PivotedTransform.Inverse() * value * this.Transform.Inverse();
    }
}
