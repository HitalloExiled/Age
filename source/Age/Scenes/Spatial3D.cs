using Age.Commands;
using Age.Core;
using Age.Numerics;

namespace Age.Scenes;

public abstract class Spatial3D : Renderable<Command3D>
{
    private CacheValue<Transform3D> transformCache;

    private Transform3D CachedParentTransform => (this.Parent as Spatial3D)?.CachedTransform ?? Transform3D.Identity;
    private Transform3D ParentTransform       => (this.Parent as Spatial3D)?.Transform ?? Transform3D.Identity;
    private Transform3D PivotedTransform      => Transform3D.Translated(this.Pivot) * this.LocalTransform * Transform3D.Translated(-this.Pivot);

    internal virtual Transform3D CachedTransform
    {
        get
        {
            if (this.transformCache.IsInvalid)
            {
                this.transformCache = new(this.PivotedTransform * this.CachedParentTransform);
            }

            return this.transformCache.Value;
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
