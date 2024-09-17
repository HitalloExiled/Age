using Age.Numerics;
using Age.Scene;

namespace Age.Elements.Layouts;

internal abstract class Layout
{
    public bool HasPendingUpdate { get; protected set; }

    public float          BaseLine   { get; internal set; } = -1;
    public Vector2<float> Offset     { get; internal set; }
    public Size<uint>     Size       { get; internal set; }
    public uint           LineHeight { get; set; }
    public bool           IsInline   { get; init; }

    public abstract Layout?    Parent { get; }
    public abstract Node       Target { get; }
    public virtual Transform2D Transform => Transform2D.Translated(this.Offset);

    public abstract void Update();

    public void RequestUpdate()
    {
        if (!this.HasPendingUpdate)
        {
            this.HasPendingUpdate = true;

            if (this.Parent != null)
            {
                this.Parent.RequestUpdate();
            }
            else if (this.Target.IsConnected)
            {
                this.Target.Tree.IsDirty = true;
            }
        }
    }
}
