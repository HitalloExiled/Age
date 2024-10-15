using Age.Numerics;
using Age.Scene;

namespace Age.Elements.Layouts;

internal abstract class Layout
{
    public bool HasPendingUpdate { get; set; }

    public Vector2<float> Offset     { get; internal set; }
    public Size<uint>     Size       { get; protected set; }
    public int            BaseLine   { get; protected set; } = -1;
    public uint           LineHeight { get; set; }

    public abstract Layout? Parent { get; }
    public abstract Node    Target { get; }

    public virtual Transform2D Transform => Transform2D.CreateTranslated(this.Offset);
    public virtual bool        Hidden { get; set; }

    public abstract void Update();

    public void RequestUpdate()
    {
        if (!this.HasPendingUpdate && !this.Hidden)
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

    public override string ToString() =>
        $"{{ Target: {this.Target} }}";
}
