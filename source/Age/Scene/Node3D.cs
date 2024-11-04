using Age.Commands;
using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Node3D : RenderNode
{
    internal static int CacheVersion { get; set; } = 1;

    private Transform3D             localTransform = new();
    private Vector3<float>          pivot;
    private CacheValue<Transform3D> transformCache;

    private Transform3D ParentTransform      => (this.Parent as Node3D)?.Transform ?? new();
    private Transform3D ParentTransformCache => (this.Parent as Node3D)?.TransformCache ?? new();
    private Transform3D PivotedTransform     => Transform3D.Translated(this.Pivot) * this.LocalTransform * Transform3D.Translated(-this.Pivot);

    protected Command? SingleCommand
    {
        get => this.Commands.Count == 1 ? this.Commands[0] : null;
        set
        {
            if (value == null)
            {
                this.Commands.Clear();
            }
            else if (this.Commands.Count == 1)
            {
                this.Commands[0] = value;
            }
            else
            {
                this.Commands.Clear();
                this.Commands.Add(value);
            }
        }
    }

    internal protected virtual Transform3D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value   = this.ParentTransformCache * this.PivotedTransform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    internal List<Command> Commands { get; init; } = [];

    public virtual Transform3D LocalTransform
    {
        get => this.localTransform;
        set => this.Set(ref this.localTransform, value);
    }

    public virtual Vector3<float> Pivot
    {
        get => this.pivot;
        set => this.Set(ref this.pivot, value);
    }

    public virtual Transform3D Transform
    {
        get => this.ParentTransform * this.PivotedTransform;
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }

    protected void Set<T>(ref T field, in T value)
    {
        if (!Equals(field, value))
        {
            field = value;
            this.OnTransformChanged();
        }
    }

    protected virtual void OnTransformChanged()
    { }
}
