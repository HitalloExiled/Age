using Age.Elements.Layouts;
using Age.Scene;
using Age.Styling;

namespace Age.Elements;

public abstract class Styleable : EventTarget
{
    internal abstract override StyledLayout Layout { get; }

    public Canvas? Canvas { get; private set; }

    public Style Style
    {
        get => this.Layout.UserStyle ??= new();
        set => this.Layout.UserStyle = value;
    }

    public StyleSheet? StyleSheet
    {
        get => this.Layout.StyleSheet;
        set => this.Layout.StyleSheet = value;
    }

    protected override void OnConnected(RenderTree renderTree)
    {
        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.Layout.HandleTargetConnected();
    }

    protected override void OnDisconnected(RenderTree renderTree)
    {
        base.OnDisconnected(renderTree);

        this.Canvas = null;

        if (!renderTree.IsDirty && !this.Layout.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Layout.HandleTargetDisconnected();
    }

    protected sealed override void OnStateChangedAdded(ElementState state) =>
        this.Layout.AddState(state);

    protected sealed override void OnStateChangedRemoved(ElementState state) =>
        this.Layout.RemoveState(state);
}
