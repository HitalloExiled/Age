using System.Text;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scene;
using Age.Styling;

using Key = Age.Platforms.Display.Key;

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

public abstract partial class Element : ContainerNode, IEnumerable<Element>
{
    private event KeyEventHandler? keyDown;
    private event KeyEventHandler? keyUp;

    public event MouseEventHandler?   Blured;
    public event MouseEventHandler?   Clicked;
    public event ContextEventHandler? Context;
    public event MouseEventHandler?   Focused;
    public event MouseEventHandler?   MouseMoved;
    public event MouseEventHandler?   MouseOut;
    public event MouseEventHandler?   MouseOver;

    public event KeyEventHandler? KeyDown
    {
        add
        {
            lock(this.elementLock)
            {
                if (this.IsConnected && keyDown == null)
                {
                    this.Tree.Window.KeyDown += this.OnKeyDown;
                }
            }

            keyDown += value;

        }
        remove
        {
            keyDown -= value;

            lock(this.elementLock)
            {
                if (this.IsConnected && keyDown == null)
                {
                    this.Tree.Window.KeyDown -= this.OnKeyDown;
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
                if (this.IsConnected && keyUp == null)
                {
                    this.Tree.Window.KeyUp += this.OnKeyUp;
                }
            }

            keyUp += value;
        }
        remove
        {
            keyUp -= value;

            lock(this.elementLock)
            {
                if (this.IsConnected && keyUp == null)
                {
                    this.Tree.Window.KeyUp -= this.OnKeyUp;
                }
            }
        }
    }

    private readonly object elementLock = new();

    private Canvas? canvas;
    private Style   style = new();
    private string? text;

    protected bool IsFocusable { get; set; }

    internal override BoxLayout Layout { get; }

    public Element? ParentElement => this.Parent as Element;

    public Element? FirstElementChild
    {
        get
        {
            for (var node = this.FirstChild; node != this.LastChild; node = node?.NextSibling)
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
            for (var node = this.NextSibling; node != this.LastChild; node = node?.NextSibling)
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
            for (var node = this.PreviousSibling; node != this.FirstChild; node = node?.PreviousSibling)
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
            for (var node = this.LastChild; node != this.FirstChild; node = node?.PreviousSibling)
            {
                if (node is Element element)
                {
                    return element;
                }
            }

            return null;
        }
    }

    public Canvas? Canvas
    {
        get => this.canvas;
        internal set
        {
            if (this.canvas != value)
            {
                this.canvas = value;

                foreach (var node in this.Traverse())
                {
                    if (node is Element element)
                    {
                        element.Canvas = value;
                    }
                }

                this.Layout.RequestUpdate();
            }
        }
    }

    public bool IsFocused { get; internal set; }

    public Style Style
    {
        get => this.style;
        set
        {
            if (this.style != value)
            {
                if (this.IsConnected)
                {
                    this.style.Changed -= this.Layout.UpdateState;
                    value.Changed += this.Layout.UpdateState;
                }

                this.style = value;

                this.Layout.UpdateState(StyleProperty.All);
            }
        }
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

                    if (this.Style.Stack == StackKind.Vertical)
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

    public override Transform2D Transform
    {
        get => base.Transform * this.Layout.Transform;
        set => this.LocalTransform = value * this.Transform.Inverse();
    }

    public Element() =>
        this.Layout = new(this);

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

    private void OnKeyDown(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key       = key,
                Holding   = !Input.IsKeyJustPressed(key),
                Modifiers = Input.GetModifiers(),

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
                Holding   = !Input.IsKeyJustPressed(key),
                Modifiers = Input.GetModifiers(),
            };

            this.keyUp?.Invoke(keyEvent);
        }
    }

    protected override void Connected(NodeTree tree)
    {
        this.style.Changed += this.Layout.UpdateState;

        if (this.keyDown != null)
        {
            tree.Window.KeyDown += this.OnKeyDown;
        }

        if (this.keyUp != null)
        {
            tree.Window.KeyUp += this.OnKeyUp;
        }
    }

    protected override void ChildAppended(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element element)
            {
                element.Canvas = this is Canvas canvas ? canvas : this.Canvas;

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
                element.Canvas = null;

                this.Layout.ElementRemoved(element);
            }

            this.Layout.ContainerNodeRemoved(containerNode);
        }
    }

    protected override void Disconnected(NodeTree tree)
    {
        this.style.Changed  -= this.Layout.UpdateState;
        tree.Window.KeyDown -= this.OnKeyDown;
        tree.Window.KeyUp   -= this.OnKeyUp;
    }

    internal void InvokeBlur(in MouseEvent mouseEvent)
    {
        if (this.IsFocusable)
        {
            this.IsFocused = false;
            this.Blured?.Invoke(mouseEvent);
        }
    }

    internal void InvokeClick(in MouseEvent mouseEvent) =>
        this.Clicked?.Invoke(mouseEvent);

    internal void InvokeContext(in ContextEvent contextEvent) =>
        this.Context?.Invoke(contextEvent);

    internal void InvokeFocus(in MouseEvent mouseEvent)
    {
        if (this.IsFocusable)
        {
            this.IsFocused = true;
            this.Focused?.Invoke(mouseEvent);
        }
    }

    internal void InvokeMouseMoved(in MouseEvent mouseEvent) =>
        this.MouseMoved?.Invoke(mouseEvent);

    internal void InvokeMouseOut(in MouseEvent mouseEvent) =>
        this.MouseOut?.Invoke(mouseEvent);

    internal void InvokeMouseOver(in MouseEvent mouseEvent) =>
        this.MouseOver?.Invoke(mouseEvent);

    public void Click() =>
        this.Clicked?.Invoke(new() { Target = this });

    public void Focus()
    {
        this.IsFocused = true;
        this.Focused?.Invoke(new() { Target = this });
    }
}
