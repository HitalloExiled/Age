using Age.Elements.Layouts;
using Age.Platforms.Display;
using Age.Scene;
using Age.Styling;
using System.Text;

using Key                  = Age.Platforms.Display.Key;
using PlatformContextEvent = Age.Platforms.Display.ContextEvent;
using PlatformMouseEvent   = Age.Platforms.Display.MouseEvent;
using AgeInput             = Age.Input;
using System.Drawing;
using Age.Numerics;

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

public abstract partial class Element : ContainerNode, IEnumerable<Element>
{
    private event InputEventHandler? input;
    private event KeyEventHandler?   keyDown;
    private event KeyEventHandler?   keyUp;
    private event MouseEventHandler? scrolled;

    public event MouseEventHandler?   Blured;
    public event MouseEventHandler?   Clicked;
    public event ContextEventHandler? Context;
    public event MouseEventHandler?   DoubleClicked;
    public event MouseEventHandler?   Focused;
    public event MouseEventHandler?   MouseMoved;
    public event MouseEventHandler?   MouseOut;
    public event MouseEventHandler?   MouseOver;

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

    private readonly Lock elementLock = new();

    #region 8-bytes
    private string? text;

    internal override BoxLayout Layout { get; }

    public Canvas? Canvas { get; private set; }
    #endregion

    #region 1-byte
    protected bool IsFocusable { get; set; }

    public bool IsFocused { get; internal set; }
    public bool IsHovered { get; internal set; }
    #endregion

    public Element? FirstElementChild
    {
        get
        {
            for (var node = this.FirstChild; node != null; node = node?.NextSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? NextElementSibling
    {
        get
        {
            for (var node = this.NextSibling; node != null; node = node?.NextSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? PreviousElementSibling
    {
        get
        {
            for (var node = this.PreviousSibling; node != null; node = node?.PreviousSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Element? LastElementChild
    {
        get
        {
            for (var node = this.LastChild; node != null; node = node?.PreviousSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
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

            foreach (var node in this.Traverse())
            {
                if (node is TextNode textNode)
                {
                    builder.Append(textNode.Value);

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
            if (value != this.text)
            {
                if (this.FirstChild is TextNode textNode)
                {
                    if (textNode != this.LastChild)
                    {
                        if (textNode.NextSibling != null && this.LastChild != null)
                        {
                            this.RemoveChildrenInRange(textNode.NextSibling, this.LastChild);
                        }
                    }

                    textNode.Value = value;
                }
                else
                {
                    this.RemoveChildren();

                    this.AppendChild(new TextNode() { Value = value });
                }

                this.text = value;

                this.Layout.RequestUpdate();
            }
        }
    }

    public Element()
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

    private MouseEvent CreateEvent(in PlatformMouseEvent mouseEvent) =>
        new()
        {
            Target    = this,
            Button    = mouseEvent.Button,
            Delta     = mouseEvent.Delta,
            KeyStates = mouseEvent.KeyStates,
            X         = mouseEvent.X,
            Y         = mouseEvent.Y,
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
            var mouseEvent = this.CreateEvent(platformMouseEvent);

            this.scrolled?.Invoke(mouseEvent);
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
            renderTree.IsDirty = true;
        }

        this.Canvas = this.ParentElement?.Canvas ?? this.Parent as Canvas;

        this.Layout.TargetConnected();
    }

    protected override void ChildAppended(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element element)
            {
                this.Layout.ElementAppended(element);
            }

            this.Layout.ContainerNodeAppended(containerNode);
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element element)
            {
                this.Layout.ElementRemoved(element);
            }

            this.Layout.ContainerNodeRemoved(containerNode);
        }
    }

    protected override void Disconnected(RenderTree renderTree)
    {
        this.Canvas = null;

        renderTree.Window.Input   -= this.OnInput;
        renderTree.Window.KeyDown -= this.OnKeyDown;
        renderTree.Window.KeyUp   -= this.OnKeyUp;

        if (!renderTree.IsDirty && !this.Layout.Hidden)
        {
            renderTree.IsDirty = true;
        }

        this.Layout.TargetDisconnected();
    }

    protected override void Indexed() =>
        this.Layout.TargetIndexed();

    internal void InvokeActivate() =>
        this.Layout.State.AddState(StyledStateManager.State.Active);

    internal void InvokeBlur(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.Layout.State.RemoveState(StyledStateManager.State.Focus);
            this.IsFocused = false;
            this.Blured?.Invoke(this.CreateEvent(platformMouseEvent));
        }
    }

    internal void InvokeClick(in PlatformMouseEvent platformMouseEvent) =>
        this.Clicked?.Invoke(this.CreateEvent(platformMouseEvent));

    internal void InvokeContext(in PlatformContextEvent platformContextEvent) =>
        this.Context?.Invoke(this.CreateEvent(platformContextEvent));

    internal void InvokeDeactivate() =>
        this.Layout.State.RemoveState(StyledStateManager.State.Active);

    internal void InvokeDoubleClick(in PlatformMouseEvent platformMouseEvent) =>
        this.DoubleClicked?.Invoke(this.CreateEvent(platformMouseEvent));

    internal void InvokeFocus(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.IsFocused = true;
            this.Layout.State.AddState(StyledStateManager.State.Focus);
            this.Focused?.Invoke(this.CreateEvent(platformMouseEvent));
        }
    }

    internal void InvokeMouseMoved(in PlatformMouseEvent platformMouseEvent) =>
        this.MouseMoved?.Invoke(this.CreateEvent(platformMouseEvent));

    internal void InvokeMouseOut(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = false;
        this.Layout.State.RemoveState(StyledStateManager.State.Hovered);
        this.Layout.TargetMouseOut();
        this.MouseOut?.Invoke(this.CreateEvent(platformMouseEvent));
    }

    internal void InvokeMouseOver(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = true;
        this.Layout.State.AddState(StyledStateManager.State.Hovered);
        this.Layout.TargetMouseOver();
        this.MouseOver?.Invoke(this.CreateEvent(platformMouseEvent));
    }

    protected override void Disposed() =>
        this.Layout.Dispose();

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

    public void Focus()
    {
        this.Layout.State.AddState(StyledStateManager.State.Focus);
        this.IsFocused = true;
        this.Focused?.Invoke(new() { Target = this });
    }

    public void Scroll(in Vector2<float> offset) =>
        this.Layout.ScrollOffset = offset;
}
