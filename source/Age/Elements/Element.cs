using Age.Core.Collections;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scene;
using Age.Styling;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Runtime.CompilerServices;

using AgeInput             = Age.Input;
using Key                  = Age.Platforms.Display.Key;
using PlatformContextEvent = Age.Platforms.Display.ContextEvent;
using PlatformMouseEvent   = Age.Platforms.Display.MouseEvent;

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

    public event Action? Activated
    {
        add => this.AddEvent(EventProperty.Activated, value);
        remove => this.RemoveEvent(EventProperty.Activated, value);
    }

    public event MouseEventHandler? Blured
    {
        add => this.AddEvent(EventProperty.Blured, value);
        remove => this.RemoveEvent(EventProperty.Blured, value);
    }

    public event MouseEventHandler? Clicked
    {
        add => this.AddEvent(EventProperty.Clicked, value);
        remove => this.RemoveEvent(EventProperty.Clicked, value);
    }

    public event ContextEventHandler? Context
    {
        add => this.AddEvent(EventProperty.Context, value);
        remove => this.RemoveEvent(EventProperty.Context, value);
    }

    public event Action? Deactivated
    {
        add => this.AddEvent(EventProperty.Deactivated, value);
        remove => this.RemoveEvent(EventProperty.Deactivated, value);
    }

    public event MouseEventHandler? DoubleClicked
    {
        add => this.AddEvent(EventProperty.DoubleClicked, value);
        remove => this.RemoveEvent(EventProperty.DoubleClicked, value);
    }

    public event MouseEventHandler? Focused
    {
        add => this.AddEvent(EventProperty.Focused, value);
        remove => this.RemoveEvent(EventProperty.Focused, value);
    }

    public event MouseEventHandler? MouseDown
    {
        add => this.AddEvent(EventProperty.MouseDown, value);
        remove => this.RemoveEvent(EventProperty.MouseDown, value);
    }

    public event MouseEventHandler? MouseMoved
    {
        add => this.AddEvent(EventProperty.MouseMoved, value);
        remove => this.RemoveEvent(EventProperty.MouseMoved, value);
    }

    public event MouseEventHandler? MouseOut
    {
        add => this.AddEvent(EventProperty.MouseOut, value);
        remove => this.RemoveEvent(EventProperty.MouseOut, value);
    }

    public event MouseEventHandler? MouseOver
    {
        add => this.AddEvent(EventProperty.MouseOver, value);
        remove => this.RemoveEvent(EventProperty.MouseOver, value);
    }

    public event MouseEventHandler? MouseUp
    {
        add => this.AddEvent(EventProperty.MouseUp, value);
        remove => this.RemoveEvent(EventProperty.MouseUp, value);
    }

    public event InputEventHandler? Input
    {
        add
        {
            lock(this.elementLock)
            {
                this.AddEvent(EventProperty.Input, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.Input += this.OnInput;
                }
            }
        }
        remove
        {
            lock(this.elementLock)
            {
                this.RemoveEvent(EventProperty.Input, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
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
                this.AddEvent(EventProperty.KeyDown, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.KeyDown += this.OnKeyDown;
                }
            }
        }
        remove
        {
            lock(this.elementLock)
            {
                this.RemoveEvent(EventProperty.KeyDown, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
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
                this.AddEvent(EventProperty.KeyUp, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.KeyUp += this.OnKeyUp;
                }
            }
        }
        remove
        {
            lock(this.elementLock)
            {
                this.RemoveEvent(EventProperty.KeyUp, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
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
                this.AddEvent(EventProperty.Scrolled, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.MouseWheel += this.OnScroll;
                }
            }
        }
        remove
        {
            lock(this.elementLock)
            {
                this.RemoveEvent(EventProperty.Scrolled, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.MouseWheel -= this.OnScroll;
                }
            }
        }
    }
    #endregion events

    private readonly Lock                               elementLock = new();
    private readonly KeyedList<EventProperty, Delegate> events      = [];

    private Action?              ActivatedEvent     => this.GetEvent<Action>(EventProperty.Activated);
    private MouseEventHandler?   BluredEvent        => this.GetEvent<MouseEventHandler>(EventProperty.Blured);
    private MouseEventHandler?   ClickedEvent       => this.GetEvent<MouseEventHandler>(EventProperty.Clicked);
    private ContextEventHandler? ContextEvent       => this.GetEvent<ContextEventHandler>(EventProperty.Context);
    private Action?              DeactivatedEvent   => this.GetEvent<Action>(EventProperty.Deactivated);
    private MouseEventHandler?   DoubleClickedEvent => this.GetEvent<MouseEventHandler>(EventProperty.DoubleClicked);
    private MouseEventHandler?   FocusedEvent       => this.GetEvent<MouseEventHandler>(EventProperty.Focused);
    private InputEventHandler?   InputEvent         => this.GetEvent<InputEventHandler>(EventProperty.Input);
    private KeyEventHandler?     KeyDownEvent       => this.GetEvent<KeyEventHandler>(EventProperty.KeyDown);
    private KeyEventHandler?     KeyUpEvent         => this.GetEvent<KeyEventHandler>(EventProperty.KeyUp);
    private MouseEventHandler?   MouseDownEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseDown);
    private MouseEventHandler?   MouseMovedEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseMoved);
    private MouseEventHandler?   MouseOutEvent      => this.GetEvent<MouseEventHandler>(EventProperty.MouseOut);
    private MouseEventHandler?   MouseOverEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseOver);
    private MouseEventHandler?   MouseUpEvent       => this.GetEvent<MouseEventHandler>(EventProperty.MouseUp);
    private MouseEventHandler?   ScrolledEvent      => this.GetEvent<MouseEventHandler>(EventProperty.Scrolled);

    protected bool IsFocusable { get; set; }

    internal protected ShadowTree? ShadowTree { get; set; }

    internal override BoxLayout Layout { get; }

    public Canvas? Canvas    { get; private set; }
    public bool    IsFocused { get; private set; }
    public bool    IsHovered { get; private set; }

    public Point<uint> Scroll
    {
        get => this.Layout.ContentOffset;
        set => this.Layout.ContentOffset = value;
    }

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

                    if (this.Layout.ComputedStyle.StackDirection == StackDirection.Vertical)
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

    protected Element()
    {
        this.Layout = new(this);
        this.Flags  = NodeFlags.IgnoreUpdates;
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

    private void AddEvent(EventProperty key, Delegate? handler, out bool added)
    {
        if (handler == null)
        {
            added = false;
            return;
        }

        added = !this.events.TryGet(key, out var @delegate);

        @delegate = Delegate.Combine(@delegate, handler);

        this.events[key] = @delegate;
    }

    private void AddEvent(EventProperty key, Delegate? handler) =>
        this.AddEvent(key, handler, out _);

    private void RemoveEvent(EventProperty key, Delegate? handler) =>
        this.RemoveEvent(key, handler, out _);

    private void RemoveEvent(EventProperty key, Delegate? handler, out bool removed)
    {
        if (handler == null)
        {
            removed = false;
            return;
        }

        this.events.TryGet(key, out var @delegate);

        @delegate = Delegate.Remove(@delegate, handler);

        if (removed = @delegate == null)
        {
            this.events.Remove(key);
        }
    }

    private T? GetEvent<T>(EventProperty key) where T : Delegate =>
        this.events.TryGet(key, out var @delegate) ? (T)@delegate : null;

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
            this.InputEvent?.Invoke(character);
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

            this.KeyDownEvent?.Invoke(keyEvent);
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

            this.KeyUpEvent?.Invoke(keyEvent);
        }
    }

    private void OnScroll(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.Layout.IsScrollable)
        {
            var mouseEvent = this.CreateEvent(platformMouseEvent, false);

            this.ScrolledEvent?.Invoke(mouseEvent);
        }
    }

    [MemberNotNull(nameof(ShadowTree))]
    protected void AttachShadowTree(bool? inheritsHostStyle = null) => this.ShadowTree = new(this, inheritsHostStyle == true);

    protected override void OnConnected(NodeTree tree)
    {
        base.OnConnected(tree);

        this.ShadowTree?.Tree = tree;
    }

    protected override void Connected(RenderTree renderTree)
    {
        if (this.events.ContainsKey(EventProperty.Input))
        {
            renderTree.Window.Input += this.OnInput;
        }

        if (this.events.ContainsKey(EventProperty.KeyDown))
        {
            renderTree.Window.KeyDown += this.OnKeyDown;
        }

        if (this.events.ContainsKey(EventProperty.KeyUp))
        {
            renderTree.Window.KeyUp += this.OnKeyUp;
        }

        if (this.events.ContainsKey(EventProperty.Scrolled))
        {
            renderTree.Window.MouseWheel += this.OnScroll;
        }

        if (!renderTree.IsDirty && !this.Layout.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Canvas = this.ComposedParentElement?.Canvas ?? this.Parent as Canvas;

        this.Layout.HandleTargetConnected();
    }

    protected override void OnChildAppended(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.Layout.HandleLayoutableAppended(layoutable);
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (this.ShadowTree == null && child is Layoutable layoutable)
        {
            this.Layout.HandleLayoutableRemoved(layoutable);
        }
    }

    protected override void OnDisconnected(NodeTree tree)
    {
        base.OnDisconnected(tree);

        this.ShadowTree?.Tree = null;
    }

    protected override void Disconnected(RenderTree renderTree)
    {
        base.Disconnected(renderTree);

        this.Canvas = null;

        renderTree.Window.Input      -= this.OnInput;
        renderTree.Window.KeyDown    -= this.OnKeyDown;
        renderTree.Window.KeyUp      -= this.OnKeyUp;
        renderTree.Window.MouseWheel -= this.OnScroll;

        if (!renderTree.IsDirty && !this.Layout.Hidden)
        {
            renderTree.MakeDirty();
        }

        this.Layout.HandleTargetDisconnected();
    }

    protected override void OnDisposed()
    {
        this.Layout.Dispose();
        this.ShadowTree?.Dispose();
    }

    protected override void OnIndexed() =>
        this.Layout.HandleTargetIndexed();

    internal ComposedTreeEnumerator GetComposedTreeEnumerator() =>
        new(this);

    internal ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator(Stack<(Slot, int)>? stack = null) =>
        new(this, stack);

    internal void InvokeActivate()
    {
        this.Layout.AddState(ElementState.Active);
        this.ActivatedEvent?.Invoke();
    }

    internal void InvokeBlur(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.Layout.RemoveState(ElementState.Focus);
            this.IsFocused = false;
            this.BluredEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeClick(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.ClickedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeContext(in PlatformContextEvent platformContextEvent) =>
        this.ContextEvent?.Invoke(this.CreateEvent(platformContextEvent));

    internal void InvokeDeactivate()
    {
        this.Layout.RemoveState(ElementState.Active);
        this.DeactivatedEvent?.Invoke();
    }

    internal void InvokeDoubleClick(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.DoubleClickedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeFocus(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.IsFocused = true;
            this.Layout.AddState(ElementState.Focus);
            this.FocusedEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeMouseDown(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseDownEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseMoved(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseMovedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseUp(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseUpEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseOut(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = false;

        if (!Layouts.Layout.IsSelectingText)
        {
            this.Layout.RemoveState(ElementState.Hovered);
        }

        this.MouseOutEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    internal void InvokeMouseOver(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = true;

        if (!Layouts.Layout.IsSelectingText)
        {
            this.Layout.AddState(ElementState.Hovered);
            this.Layout.HandleTargetMouseOver();
        }

        this.MouseOverEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
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
        this.Layout.RemoveState(ElementState.Focus);
        this.BluredEvent?.Invoke(new() { Target = this });
    }

    public void Click()
    {
        this.Layout.AddState(ElementState.Active);
        this.ClickedEvent?.Invoke(new() { Target = this });
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
        this.Layout.AddState(ElementState.Focus);
        this.IsFocused = true;
        this.FocusedEvent?.Invoke(new() { Target = this });
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
