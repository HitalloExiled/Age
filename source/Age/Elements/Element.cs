using System.Text;
using Age.Core;
using Age.Elements.Layouts;
using Age.Numerics;
using Age.Scene;
using Age.Styling;

namespace Age.Elements;

public delegate void ContextEventHandler(in ContextEvent mouseEvent);
public delegate void MouseEventHandler(in MouseEvent mouseEvent);

public abstract partial class Element : ContainerNode, IEnumerable<Element>
{
    public event MouseEventHandler?   Blured;
    public event MouseEventHandler?   Clicked;
    public event ContextEventHandler? Context;
    public event MouseEventHandler?   Focused;
    public event MouseEventHandler?   MouseMoved;
    public event MouseEventHandler?   MouseOut;
    public event MouseEventHandler?   MouseOver;

    private Canvas?                 canvas;
    private Style                   style = new();
    private string?                 text;
    private CacheValue<Transform2D> transformCache;

    internal protected override Transform2D TransformCache
    {
        get
        {
            if (this.transformCache.Version != CacheVersion)
            {
                this.transformCache = new()
                {
                    Value = base.TransformCache * this.Layout.Transform,
                    Version = CacheVersion
                };
            }

            return this.transformCache.Value;
        }
    }

    internal override BoxLayout Layout { get; }

    internal bool HasPendingUpdate { get; set; }

    public Element? ParentElement => this.Parent as Element;

    public Element? FirstElementChild { get; private set; }
    public Element? LastElementChild  { get; private set; }

    public Element? PreviousElementSibling { get; private set; }
    public Element? NextElementSibling     { get; private set; }

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
                this.style.Changed -= this.Layout.StyleChanged;
                this.style = value;
                this.style.Changed += this.Layout.StyleChanged;

                this.Layout.StyleChanged();
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

    public Element()
    {
        this.Layout = new(this);
        this.style.Changed += this.Layout.StyleChanged;
    }

    ~Element() =>
        this.style.Changed -= this.Layout.StyleChanged;

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        for (var childElement = this.FirstElementChild; childElement != null; childElement = childElement.NextElementSibling)
        {
            yield return childElement;
        }
    }

    protected override void ChildAppended(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element element)
            {
                element.Canvas = this is Canvas canvas ? canvas : this.Canvas;

                if (this.LastElementChild != null)
                {
                    this.LastElementChild.NextElementSibling = element;
                    element.PreviousElementSibling = this.LastElementChild;

                    this.LastElementChild = element;
                }
                else
                {
                    this.FirstElementChild = this.LastElementChild = element;
                }

                this.Layout.AddDependent(element);
            }

            this.Layout.IncreaseRenderableNodes();
        }
    }

    protected override void ChildRemoved(Node child)
    {
        if (child is ContainerNode containerNode)
        {
            if (containerNode is Element element)
            {
                element.Canvas = null;

                if (element == this.FirstElementChild)
                {
                    this.FirstElementChild = element.NextElementSibling;
                }

                if (element == this.LastElementChild)
                {
                    this.LastElementChild = element.PreviousElementSibling;
                }

                if (element.PreviousElementSibling != null)
                {
                    element.PreviousElementSibling.NextElementSibling = element.NextElementSibling;

                    if (element.NextElementSibling != null)
                    {
                        element.NextElementSibling.PreviousElementSibling = element.PreviousElementSibling.NextElementSibling;
                    }
                }
                else if (element.NextElementSibling != null)
                {
                    element.NextElementSibling.PreviousElementSibling = null;
                }

                element.PreviousElementSibling = null;
                element.NextElementSibling = null;

                this.Layout.RemoveDependent(element);
            }

            this.Layout.DecreaseRenderableNodes();

        }
    }

    internal void InvokeBlur(in MouseEvent mouseEvent)
    {
        this.IsFocused = false;
        this.Blured?.Invoke(mouseEvent);
    }

    internal void InvokeClick(in MouseEvent mouseEvent) =>
        this.Clicked?.Invoke(mouseEvent);

    internal void InvokeContext(in ContextEvent contextEvent) =>
        this.Context?.Invoke(contextEvent);

    internal void InvokeFocus(in MouseEvent mouseEvent)
    {
        this.IsFocused = true;
        this.Focused?.Invoke(mouseEvent);
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
