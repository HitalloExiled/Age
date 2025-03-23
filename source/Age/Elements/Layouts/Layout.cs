using Age.Core;
using Age.Numerics;
using Age.Scene;

namespace Age.Elements.Layouts;

internal abstract class Layout : Disposable
{
    protected virtual StencilLayer? ContentStencilLayer { get; }

    public bool IsDirty { get; private set; }

    public int        BaseLine  { get; protected set; } = -1;
    public Size<uint> Boundings { get; protected set; }

    public Vector2<float> Offset { get; internal set; }

    public uint LineHeight { get; set; }

    public BoxLayout? Parent => this.Target.ComposedParentElement?.Layout;

    public virtual bool          Hidden       { get; set; }
    public virtual StencilLayer? StencilLayer { get; set; }

    public virtual Transform2D Transform => Transform2D.CreateTranslated(this.Offset);

    public abstract bool       IsParentDependent { get; }
    public abstract Layoutable Target            { get; }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            this.Disposed();
        }
    }

    protected abstract void Disposed();

    public void RequestUpdate(bool affectsBoundings)
    {
        if (!this.IsDirty && !this.Hidden)
        {
            this.MakeDirty();

            if ((this.IsParentDependent || affectsBoundings) && this.Parent != null)
            {
                this.Parent.RequestUpdate(affectsBoundings);
            }
            else if (this.Target.Tree is RenderTree renderTree)
            {
                if (this.Target.Parent != renderTree.Root)
                {
                    renderTree.AddDeferredUpdate(this.Update);
                }
                else
                {
                    renderTree.MakeDirty();
                }
            }
        }
    }

    public void MakeDirty() =>
        this.IsDirty = true;

    public void MakePristine() =>
        this.IsDirty = false;

    public virtual void TargetConnected() =>
        this.StencilLayer = this.Parent?.ContentStencilLayer;

    public virtual void TargetDisconnected() =>
        this.StencilLayer = null;

    public override string ToString() =>
        $"{{ Target: {this.Target} }}";

    public abstract void Update();
}
