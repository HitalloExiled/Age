using Age.Commands;
using Age.Core;
using Age.Numerics;

namespace Age.Scenes;

public abstract class Spatial2D : Spatial<Command2D, Matrix3x2<float>>
{
    private CacheValue<Matrix3x2<float>> transformCache;

    private protected Matrix3x2<float> CachedCompositeParentMatrix => (this.CompositeParent as Spatial2D)?.CachedMatrix ?? Matrix3x2<float>.Identity;
    private protected Matrix3x2<float> CompositeParentMatrix       => (this.CompositeParent as Spatial2D)?.Transform ?? Matrix3x2<float>.Identity;

    public sealed override Matrix3x2<float> CachedMatrix
    {
        get
        {
            if (this.transformCache.IsInvalid)
            {
                this.transformCache = new(this.LocalTransform.Matrix * this.CachedCompositeParentMatrix);
            }

            return this.transformCache.Value;
        }
    }

    public Transform2D LocalTransform { get; set; } = Transform2D.Identity;

    public new Scene2D? Scene => base.Scene as Scene2D;

    public Transform2D Transform
    {
        get => this.LocalTransform.Matrix * this.CompositeParentMatrix;
        set => this.LocalTransform = value.Matrix * this.CompositeParentMatrix.Inverse();
    }

    public sealed override Matrix3x2<float> Matrix => this.Transform.Matrix;
}
