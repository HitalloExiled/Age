using Age.Commands;
using Age.Core;
using Age.Numerics;

namespace Age.Scene;

public abstract class Node2D : Node
{
    internal static int CacheVersion { get; set; } = 1;

    private Transform2D             localTransform = new();
    private Vector2<float>          pivot;
    private CacheValue<Transform2D> transformCache;

    private Transform2D ParentTransform      => (this.Parent as Node2D)?.Transform ?? new();
    private Transform2D ParentTransformCache => (this.Parent as Node2D)?.TransformCache ?? new();
    private Transform2D PivotedTransform     => Transform2D.Translated(this.Pivot) * this.LocalTransform * Transform2D.Translated(-this.Pivot);

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

    internal protected virtual Transform2D TransformCache
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

    public virtual Transform2D LocalTransform
    {
        get => this.localTransform;
        set => this.Set(ref this.localTransform, value, this.TransformChanged);
    }

    public virtual Vector2<float> Pivot
    {
        get => this.pivot;
        set => this.Set(ref this.pivot, value, this.TransformChanged);
    }

    public virtual Transform2D Transform
    {
        get => this.ParentTransform * this.PivotedTransform;
        set => this.LocalTransform = value * this.ParentTransform.Inverse();
    }

    protected void Set<T>(ref T field, in T value, Action callback)
    {
        if (!Equals(field, value))
        {
            field = value;

            callback.Invoke();

            if (this.IsConnected)
            {
                this.Tree.IsDirty = true;
            }
        }
    }

    protected virtual void TransformChanged()
    { }
}
