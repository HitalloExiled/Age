using System.Text;
using Age.Core;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing.Elements;

namespace Age.Rendering.Drawing;

public abstract class Element : Node2D, IEnumerable<Element>
{
    private readonly Style style = new();

    private Canvas? canvas;
    private bool    hasPendingUpdate;
    private string? text;

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
            this.canvas = value;

            foreach (var item in this.Enumerate<Element>())
            {
                item.Canvas = value;
            }

            this.RequestUpdate();
        }
    }

    public Style Style
    {
        get => this.style;
        set
        {
            this.style.Update(value);

            if (this.IsConnected && this.style.HasChanges)
            {
                this.RequestUpdate();
            }
        }
    }

    public string? Text
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var item in this.Traverse<TextNode>(true))
            {
                builder.Append(item.Value);
            }

            return builder.ToString();
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

                if (this.IsConnected)
                {
                    this.RequestUpdate();
                }
            }
        }
    }

    IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
    {
        for (var childElement = this.FirstElementChild; childElement != null; childElement = childElement.NextElementSibling)
        {
            yield return childElement;
        }
    }

    private void DrawBorder()
    {
        RectDrawCommand command;

        if (this.Commands.Count == 0)
        {
            command = new()
            {
                SampledTexture = new(
                    Container.Singleton.TextureStorage.DefaultTexture,
                    Container.Singleton.TextureStorage.DefaultSampler,
                    UVRect.Normalized
                ),
            };

            this.Commands.Add(command);
        }
        else
        {
            command = (RectDrawCommand)this.Commands[0];
        }

        command.Rect = new(this.Size, this.Transform.Position);
        command.Color = Color.Cyan;
        Logger.Trace($"Drawing Border: {this}");
    }

    private void RequestUpdate()
    {
        Logger.Trace($"Requesting Update: {this}");
        if (this.canvas != null && !this.hasPendingUpdate)
        {
            this.hasPendingUpdate = true;
            this.canvas.RequestUpdate(this);
            Container.Singleton.RenderingService.RequestDraw();
            Logger.Trace($"Update Requested: {this}");
        }
    }

    protected override void OnChildAppended(Node child)
    {
        if (child is Element childElement)
        {
            if (this.LastElementChild != null)
            {
                this.LastElementChild.NextElementSibling = childElement;
                childElement.PreviousElementSibling = this.LastElementChild;

                this.LastElementChild = childElement;
            }
            else
            {
                this.FirstElementChild = this.LastElementChild = childElement;
            }

            childElement.Canvas = this.Canvas;
            this.RequestUpdate();
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Element elementChild)
        {
            if (elementChild == this.FirstElementChild)
            {
                this.FirstElementChild = elementChild.NextElementSibling;
            }

            if (elementChild == this.LastElementChild)
            {
                this.FirstElementChild = elementChild.PreviousElementSibling;
            }

            if (elementChild.PreviousElementSibling != null)
            {
                elementChild.PreviousElementSibling.NextElementSibling = elementChild.NextElementSibling;

                if (elementChild.NextElementSibling != null)
                {
                    elementChild.NextElementSibling.PreviousElementSibling = elementChild.PreviousElementSibling.NextElementSibling;
                }
            }
            else if (elementChild.NextElementSibling != null)
            {
                elementChild.NextElementSibling.PreviousElementSibling = null;
            }

            elementChild.PreviousElementSibling = null;
            elementChild.NextElementSibling     = null;
        }
    }

    protected override void OnBoundsChanged() =>
        this.RequestUpdate();

    internal void UpdateLayout()
    {
        Logger.Trace($"Updating Layout: {this}");
        var size     = new Size<float>();
        var previous = new Rect<float>();

        foreach (var child in this.Enumerate<Node2D>())
        {
            if (child is TextNode textNode)
            {
                textNode.Redraw();
            }

            child.Transform = child.Transform with
            {
                Position = this.Transform.Position + new Vector2<float>(previous.Size.Width + previous.Position.X, 0)
            };

            size.Width += child.Size.Width;
            size.Height = float.Max(size.Height, child.Size.Height);

            previous = new(child.Size, child.Transform.Position);
        }

        this.Size = size;

        this.DrawBorder();

        this.hasPendingUpdate = false;
        Logger.Trace($"Layout Updated: {this}");
    }
}
