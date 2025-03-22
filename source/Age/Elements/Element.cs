using Age.Elements.Layouts;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scene;
using Age.Styling;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Key                  = Age.Platforms.Display.Key;
using PlatformContextEvent = Age.Platforms.Display.ContextEvent;
using PlatformMouseEvent   = Age.Platforms.Display.MouseEvent;
using AgeInput             = Age.Input;
using Age.Core.Extensions;
using System.Runtime.CompilerServices;

namespace Age.Elements;

public struct KeyEvent
{
    public Key       Key;
    public KeyStates Modifiers;
    public bool      Holding;
}

public delegate void ContextEventHandler(in ContextEvent mouseEvent);
public delegate void MouseEventHandler(in MouseEvent mouseEvent);
public delegate void KeyEventHandler(in KeyEvent keyEvent);
public delegate void InputEventHandler(char keyEvent);

public abstract partial class Element : Layoutable, IComparable<Element>, IEnumerable<Element>
{
    #region events
    private event InputEventHandler? input;
    private event KeyEventHandler?   keyDown;
    private event KeyEventHandler?   keyUp;
    private event MouseEventHandler? scrolled;

    public event Action?              Activated;
    public event MouseEventHandler?   Blured;
    public event MouseEventHandler?   Clicked;
    public event ContextEventHandler? Context;
    public event Action?              Deactivated;
    public event MouseEventHandler?   DoubleClicked;
    public event MouseEventHandler?   Focused;
    public event MouseEventHandler?   MouseDown;
    public event MouseEventHandler?   MouseMoved;
    public event MouseEventHandler?   MouseOut;
    public event MouseEventHandler?   MouseOver;
    public event MouseEventHandler?   MouseUp;

    public event InputEventHandler? Input
    {
        add
        {
            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && keyDown == null)
                {
                    renderTree.Window.Input += this.OnInput;
                }
            }

            input += value;

        }
        remove
        {
            input -= value;

            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && keyDown == null)
                {
                    renderTree.Window.Input -= this.OnInput;
                }
            }
        }
    }

    public event KeyEventHandler? KeyDown
    {
        add
        {
            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && keyDown == null)
                {
                    renderTree.Window.KeyDown += this.OnKeyDown;
                }
            }

            keyDown += value;

        }
        remove
        {
            keyDown -= value;

            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && keyDown == null)
                {
                    renderTree.Window.KeyDown -= this.OnKeyDown;
                }
            }
        }
    }

    public event KeyEventHandler? KeyUp
    {
        add
        {
            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && keyUp == null)
                {
                    renderTree.Window.KeyUp += this.OnKeyUp;
                }
            }

            keyUp += value;
        }
        remove
        {
            keyUp -= value;

            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && keyUp == null)
                {
                    renderTree.Window.KeyUp -= this.OnKeyUp;
                }
            }
        }
    }

    public event MouseEventHandler? Scrolled
    {
        add
        {
            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && scrolled == null)
                {
                    renderTree.Window.MouseWhell += this.OnScroll;
                }
            }

            scrolled += value;
        }
        remove
        {
            scrolled -= value;

            lock(this.elementLock)
            {
                if (this.Tree is RenderTree renderTree && scrolled == null)
                {
                    renderTree.Window.MouseWhell -= this.OnScroll;
                }
            }
        }
    }
    #endregion events

    #region 8-bytes
    private readonly Lock elementLock = new();

    internal protected ShadowTree? ShadowTree { get; set; }

    internal Dictionary<string, List<Node>> WaitingSlots { get; } = [];
    internal Dictionary<string, Slot> Slots              { get; } = [];

    internal override BoxLayout Layout { get; }

    public Canvas? Canvas { get; private set; }

    public string? Slot
    {
        get;
        set
        {
            if (field != value)
            {
                if (this.Parent is Element parentElement && parentElement.ShadowTree != null)
                {
                    this.UnassignSlot(parentElement, field ?? "");
                    this.AssignSlot(parentElement, value ?? "");
                }

                field = value;
            }
        }
    }
    #endregion

    #region 1-byte
    protected bool IsFocusable { get; set; }

    public bool IsFocused { get; private set; }
    public bool IsHovered { get; private set; }
    #endregion

    public Point<uint> Scroll
    {
        get => this.Layout.ContentOffset;
        set => this.Layout.ContentOffset = value;
    }

    public Style Style
    {
        get => this.Layout.State.UserStyle ??= new();
        set => this.Layout.State.UserStyle = value;
    }

    public StyledStates? States
    {
        get => this.Layout.State.Styles;
        set => this.Layout.State.Styles = value;
    }

    public string? Text
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var node in this.GetComposedTreeTraversalEnumerator())
            {
                if (node is Text text)
                {
                    builder.Append(text.Buffer);

                    if (this.Layout.State.Style.Stack == StackKind.Vertical)
                    {
                        builder.Append('\n');
                    }
                }
            }

            return builder.ToString().TrimEnd();
        }
        set
        {
            if (this.FirstChild is Text text)
            {
                if (text != this.LastChild)
                {
                    if (text.NextSibling != null && this.LastChild != null)
                    {
                        this.RemoveChildrenInRange(text.NextSibling, this.LastChild);
                    }
                }

                text.Value = value;
            }
            else
            {
                this.RemoveChildren();

                this.AppendChild(new Text(value));
            }

            this.Layout.RequestUpdate(true);
        }
    }

    public Element()
    {
        this.Layout = new(this);
        this.Flags  = NodeFlags.IgnoreUpdates;
    }

    protected Element(bool useShadowTree) : this()
    {
        if (useShadowTree)
        {
            this.ShadowTree = new(this);
        }
    }

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        foreach (var node in this)
        {
            if (node is Element element)
            {
                yield return element;
            }
        }
    }

    private void AssignSlot(Element parent, string name)
    {
        if (!parent.WaitingSlots.TryGetValue(name, out var waitingSlots))
        {
            parent.WaitingSlots[name] = waitingSlots = [];
        }

        if (parent.Slots.TryGetValue(name, out var slot))
        {
            waitingSlots.Remove(this);

            slot.Assign(this);
        }
        else
        {
            waitingSlots.Add(this);
        }
    }

    private void UnassignSlot(Element parent, string name)
    {
        if (parent.Slots.TryGetValue(name, out var slot) == true)
        {
            slot.Unassign(this);
        }
    }

    private MouseEvent CreateEvent(in PlatformMouseEvent mouseEvent, bool indirect) =>
        new()
        {
            Target        = this,
            Button        = mouseEvent.Button,
            Delta         = mouseEvent.Delta,
            KeyStates     = mouseEvent.KeyStates,
            PrimaryButton = mouseEvent.PrimaryButton,
            X             = mouseEvent.X,
            Y             = mouseEvent.Y,
            Indirect      = indirect,
        };

    private ContextEvent CreateEvent(in PlatformContextEvent platformContextEvent) =>
        new()
        {
            Target  = this,
            X       = platformContextEvent.X,
            Y       = platformContextEvent.Y,
            ScreenX = platformContextEvent.ScreenX,
            ScreenY = platformContextEvent.ScreenY,
        };

    private void OnInput(char character)
    {
        if (this.IsFocused)
        {
            this.input?.Invoke(character);
        }
    }

    private void OnKeyDown(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key       = key,
                Holding   = !AgeInput.IsKeyJustPressed(key),
                Modifiers = AgeInput.GetModifiers(),
            };

            this.keyDown?.Invoke(keyEvent);
        }
    }

    private void OnKeyUp(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key       = key,
                Holding   = !AgeInput.IsKeyJustPressed(key),
                Modifiers = AgeInput.GetModifiers(),
            };

            this.keyUp?.Invoke(keyEvent);
        }
    }

    private void OnScroll(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.Layout.IsScrollable)
        {
            var mouseEvent = this.CreateEvent(platformMouseEvent, false);

            this.scrolled?.Invoke(mouseEvent);
        }
    }

    [MemberNotNull(nameof(ShadowTree))]
    protected void AttachShadowTree() => this.ShadowTree = new(this);

    protected override void Connected(NodeTree tree)
    {
        base.Connected(tree);

        if (this.ShadowTree != null)
        {
            this.ShadowTree.Tree = tree;
        }

        if (this.Parent is Element parentElement && parentElement.ShadowTree != null)
        {
            this.AssignSlot(parentElement, this.Slot ?? "");
        }
    }

    protected override void Connected(RenderTree renderTree)
    {
        if (this.input != null)
        {
            renderTree.Window.Input += this.OnInput;
        }

        if (this.keyDown != null)
        {
            renderTree.Window.KeyDown += this.OnKeyDown;
        }

        if (this.keyUp != null)
        {
            renderTree.Window.KeyUp += this.OnKeyUp;
        }

        if (this.scrolled != null)
        {
            renderTree.Window.MouseWhell += this.OnScroll;
        }

        if (!renderTree.IsDirty && !this.Layout.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.Layout.TargetConnected();
    }

    protected override void ChildAppended(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.Layout.LayoutableAppended(layoutable);
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.Layout.LayoutableRemoved(layoutable);
        }
    }

    protected override void Disconnected(NodeTree tree)
    {
        base.Disconnected(tree);

        if (this.ShadowTree != null)
        {
            this.ShadowTree.Tree = null;
        }
    }

    protected override void Disconnected(RenderTree renderTree)
    {
        base.Disconnected(renderTree);

        this.Canvas = null;

        renderTree.Window.Input   -= this.OnInput;
        renderTree.Window.KeyDown -= this.OnKeyDown;
        renderTree.Window.KeyUp   -= this.OnKeyUp;

        if (!renderTree.IsDirty && !this.Layout.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Layout.TargetDisconnected();
    }

    protected override void Indexed() =>
        this.Layout.TargetIndexed();

    protected override void Removed(Node parent)
    {
        base.Removed(parent);

        if (parent is Element parentElement && parentElement.ShadowTree != null)
        {
            this.UnassignSlot(parentElement, this.Slot ?? "");
        }
    }

    internal ComposedTreeEnumerator GetComposedTreeEnumerator() =>
        new(this);

    internal ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator(Stack<(Slot, int)>? stack = null) =>
        new(this, stack);

    internal void InvokeActivate()
    {
        this.Layout.State.AddState(StyledStateManager.State.Active);
        this.Activated?.Invoke();
    }

    internal void InvokeBlur(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.Layout.State.RemoveState(StyledStateManager.State.Focus);
            this.IsFocused = false;
            this.Blured?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeClick(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.Clicked?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeContext(in PlatformContextEvent platformContextEvent) =>
        this.Context?.Invoke(this.CreateEvent(platformContextEvent));

    internal void InvokeDeactivate()
    {
        this.Layout.State.RemoveState(StyledStateManager.State.Active);
        this.Deactivated?.Invoke();
    }

    internal void InvokeDoubleClick(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.DoubleClicked?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeFocus(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.IsFocused = true;
            this.Layout.State.AddState(StyledStateManager.State.Focus);
            this.Focused?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeMouseDown(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseDown?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseMoved(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseMoved?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseUp(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseUp?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseOut(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = false;
        this.Layout.State.RemoveState(StyledStateManager.State.Hovered);
        this.Layout.TargetMouseOut();
        this.MouseOut?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    internal void InvokeMouseOver(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = true;
        this.Layout.State.AddState(StyledStateManager.State.Hovered);
        this.Layout.TargetMouseOver();
        this.MouseOver?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    protected override void Disposed()
    {
        this.Layout.Dispose();
        this.ShadowTree?.Dispose();
    }

    internal int GetEffectiveDepth()
    {
        var depth = 0;

        var node = this.EffectiveParentElement;

        while (node != null)
        {
            depth++;
            node = node.EffectiveParentElement;
        }

        return depth;
    }

    public void Blur()
    {
        this.IsFocused = false;
        this.Layout.State.RemoveState(StyledStateManager.State.Focus);
        this.Blured?.Invoke(new() { Target = this });
    }

    public void Click()
    {
        this.Layout.State.AddState(StyledStateManager.State.Active);
        this.Clicked?.Invoke(new() { Target = this });
    }

    public int CompareTo(Element? other)
    {
        if (other == null)
        {
            return 1;
        }
        else if (this == other)
        {
            return 0;
        }

        var left  = this;
        var right = other;

        var leftParent  = left.EffectiveParentElement;
        var rightParent = right.EffectiveParentElement;

        if (leftParent != rightParent)
        {
            var leftDepth  = getDepth(leftParent);
            var rightDepth = getDepth(rightParent);

            while (leftDepth > rightDepth)
            {
                leftParent = left.EffectiveParentElement;

                if (leftParent == right)
                {
                    return 1;
                }

                left = leftParent!;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                rightParent = right.EffectiveParentElement;

                if (rightParent == left)
                {
                    return -1;
                }

                right = rightParent!;
                rightDepth--;
            }

            leftParent  = left.EffectiveParentElement;
            rightParent = right.EffectiveParentElement;

            while (leftParent != rightParent)
            {
                left  = leftParent!;
                right = rightParent!;

                leftParent  = left.EffectiveParentElement;
                rightParent = right.EffectiveParentElement;
            }
        }

        if (leftParent == rightParent)
        {
            if (leftParent == null)
            {
                throw new InvalidOperationException("Can't compare an root node to another");
            }

            if (left.Parent == right.Parent)
            {
                if (left == right.NextSibling)
                {
                    return 1;
                }

                if (left != right.PreviousSibling)
                {
                    for (var node = left!.PreviousSibling; node != null; node = node?.PreviousSibling)
                    {
                        if (node == right)
                        {
                            return 1;
                        }
                    }
                }
            }
            else if (right.Parent is ShadowTree)
            {
                return 1;
            }
        }

        return -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int getDepth(Element? parentElement) =>
            parentElement == null ? 0 : parentElement.GetEffectiveDepth() + 1;
    }

    public void Focus()
    {
        this.Layout.State.AddState(StyledStateManager.State.Focus);
        this.IsFocused = true;
        this.Focused?.Invoke(new() { Target = this });
    }

    public BoxModel GetBoxModel()
    {
        var boundings = this.GetBoundings();

        var padding = this.Layout.Padding;
        var border  = this.Layout.Border;
        var content = this.Layout.Content;
        var margin  = this.Layout.Margin;

        return new()
        {
            Margin    = margin,
            Boundings = boundings,
            Border    = border,
            Padding   = padding,
            Content   = content,
        };
    }

    public void ScrollTo(in Rect<int> boundings)
    {
        if (!this.Layout.CanScrollX || !this.Layout.CanScrollY)
        {
            return;
        }

        var boxModel = this.GetBoxModel();

        var boundsLeft   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
        var boundsRight  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
        var boundsTop    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
        var boundsBottom = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

        var scroll = this.Scroll;

        if (this.Layout.CanScrollX)
        {
            if (boundings.Left < boundsLeft)
            {
                var characterLeft = boundings.Left + scroll.X;

                scroll.X = (uint)(characterLeft - boundsLeft);
            }
            else if (boundings.Right > boundsRight)
            {
                var characterRight = boundings.Right + scroll.X;

                scroll.X = (uint)(characterRight - boundsRight);
            }
        }

        if (this.Layout.CanScrollY)
        {
            if (boundings.Top < boundsTop)
            {
                var characterTop = boundings.Top + scroll.Y;

                scroll.Y = (uint)(characterTop - boundsTop);
            }
            else if (boundings.Bottom > boundsBottom)
            {
                var characterBottom = boundings.Bottom + scroll.Y;

                scroll.Y = (uint)(characterBottom - boundsBottom);
            }
        }

        this.Scroll = scroll;
    }

    public void ScrollTo(Element element) =>
        this.ScrollTo(element.GetBoundings());
}
