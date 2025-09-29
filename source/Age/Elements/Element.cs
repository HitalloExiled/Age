using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Age.Elements.Enumerators;
using Age.Scenes;
using Age.Styling;

namespace Age.Elements;

public abstract partial class Element : Layoutable, IComparable<Element>, IEnumerable<Element>
{
    internal protected ShadowTree? ShadowTree { get; set; }

    public Canvas? Canvas { get; private set; }

    public string? InnerText
    {
        get
        {
            var builder = new StringBuilder();

            foreach (var node in this.GetComposedTreeTraversalEnumerator())
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

    private ComposedElementEnumerator GetComposedElementEnumerator() =>
        new(this);

    [MemberNotNull(nameof(ShadowTree))]
    protected void AttachShadowTree(bool? inheritsHostStyle = null) =>
        this.ShadowTree = new(this, inheritsHostStyle == true);

    internal ComposedTreeEnumerator GetComposedTreeEnumerator() =>
        new(this);

    internal ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator(Stack<(Slot, int)>? stack = null, Action<Element>? parentCallback = null) =>
        new(this, stack, parentCallback);

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
}
