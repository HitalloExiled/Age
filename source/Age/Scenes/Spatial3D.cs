using Age.Commands;
using Age.Core;
using Age.Numerics;

namespace Age.Scenes;

public abstract class Spatial3D : Spatial<Command3D, Matrix4x4<float>>
{
    private CacheValue<Matrix4x4<float>> transformCache;

    private Matrix4x4<float> CachedCompositeParentMatrix => (this.CompositeParent as Spatial3D)?.CachedMatrix ?? Matrix4x4<float>.Identity;
    private Matrix4x4<float> CompositeParentMatrix       => (this.CompositeParent as Spatial3D)?.Matrix ?? Matrix4x4<float>.Identity;
    private Matrix4x4<float> PivotedMatrix               => Matrix4x4<float>.Translated(this.Pivot) * this.LocalTransform.Matrix * Matrix4x4<float>.Translated(-this.Pivot);

    public sealed override Matrix4x4<float> CachedMatrix
    {
        get
        {
            if (this.transformCache.IsInvalid)
            {
                this.transformCache = new(this.PivotedMatrix * this.CachedCompositeParentMatrix);
            }

            return this.transformCache.Value;
        }
    }

    public Transform3D LocalTransform { get; set; } = Transform3D.Identity;

    public sealed override Matrix4x4<float> Matrix => this.Transform;

    public Vector3<float> Pivot { get; set; }

    public new Scene3D? Scene => base.Scene as Scene3D;

    public Transform3D Transform
    {
        get => this.PivotedMatrix * this.CompositeParentMatrix;
        set => this.LocalTransform = Matrix4x4<float>.Translated(-this.Pivot) * value.Matrix * this.CompositeParentMatrix.Inverse() * Matrix4x4<float>.Translated(this.Pivot);
    }
}
