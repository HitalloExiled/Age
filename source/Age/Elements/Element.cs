using System.Text;
using Age.Styling;

namespace Age.Elements;

public abstract partial class Element : Layoutable
{
    public string? InnerText
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var node in this.GetCompositeTraversalEnumerator())
            {
                if (node is Text text)
                {
                    builder.Append(text.Buffer);

                    if (this.ComputedStyle.StackDirection == StackDirection.Vertical)
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
                if (text != this.LastChild && text.NextSibling != null && this.LastChild != null)
                {
                    this.DetachChildrenInRange(text.NextSibling, this.LastChild);
                }

                text.Value = value;
            }
            else
            {
                this.DetachChildren();

                this.AppendChild(new Text(value));
            }

            this.RequestUpdate(true);
        }
    }

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

    protected Element() =>
        this.SuspendUpdates();

    public BoxModel GetBoxModel()
    {
        var boundings = this.GetUpdatedBoundings();

        var padding = this.padding;
        var border  = this.border;
        var content = this.content;
        var margin  = this.margin;

        return new()
        {
            Margin    = margin,
            Boundings = boundings,
            Border    = border,
            Padding   = padding,
            Content   = content,
        };
    }
}
