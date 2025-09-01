using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Age.Elements.Enumerators;
using Age.Scene;
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
                    this.RemoveChildrenInRange(text.NextSibling, this.LastChild);
                }

                text.Value = value;
            }
            else
            {
                this.RemoveChildren();

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

    protected Element() => this.NodeFlags = NodeFlags.IgnoreUpdates;

    private ComposedElementEnumerator GetComposedElementEnumerator() =>
        new(this);

    [MemberNotNull(nameof(ShadowTree))]
    protected void AttachShadowTree(bool? inheritsHostStyle = null) =>
        this.ShadowTree = new(this, inheritsHostStyle == true);

    internal static Element? GetCommonComposedAncestor(Element left, Element right)
    {
        var leftComposedParentElement  = left.ComposedParentElement;
        var rightComposedParentElement = right.ComposedParentElement;

        if (leftComposedParentElement == rightComposedParentElement)
        {
            return leftComposedParentElement;
        }
        else if (left == rightComposedParentElement)
        {
            return left;
        }
        else if (leftComposedParentElement == right)
        {
            return right;
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            var currentLeft  = leftComposedParentElement;
            var currentRight = rightComposedParentElement;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft  = currentLeft.ComposedParentElement;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.ComposedParentElement;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.ComposedParentElement!;
                leftDepth--;
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.ComposedParentElement!;
                rightDepth--;
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.ComposedParentElement;
                currentRight = currentRight.ComposedParentElement;

                if (currentLeft == null || currentRight == null)
                {
                    return null;
                }
            }

            return currentLeft;
        }
    }

    internal static ComposedPath GetComposedPathBetween(Element left, Element right)
    {
        var leftToAncestor  = new List<Element>();
        var rightToAncestor = new List<Element>();

        GetComposedPathBetween(leftToAncestor, rightToAncestor, left, right);

        return new(leftToAncestor, rightToAncestor);
    }

    internal static void GetComposedPathBetween(List<Element> leftToAncestor, List<Element> rightToAncestor, Element left, Element right)
    {
        const string ERROR_MESSAGE = "The specified elements do not share a common ancestor in the composed tree.";

        leftToAncestor.Clear();
        rightToAncestor.Clear();

        leftToAncestor.Add(left);
        rightToAncestor.Add(right);

        var leftComposedParentElement  = left.ComposedParentElement;
        var rightComposedParentElement = right.ComposedParentElement;

        if (leftComposedParentElement == rightComposedParentElement)
        {
            if (leftComposedParentElement == null)
            {
                throw new InvalidOperationException(ERROR_MESSAGE);
            }

            leftToAncestor.Add(leftComposedParentElement);
            rightToAncestor.Add(leftComposedParentElement);
        }
        else if (left == rightComposedParentElement)
        {
            rightToAncestor.Add(left);
        }
        else if (leftComposedParentElement == right)
        {
            leftToAncestor.Add(right);
        }
        else
        {
            var leftDepth  = 0;
            var rightDepth = 0;

            var currentLeft  = leftComposedParentElement;
            var currentRight = rightComposedParentElement;

            while (currentLeft != null)
            {
                leftDepth++;
                currentLeft = currentLeft.ComposedParentElement;
            }

            while (currentRight != null)
            {
                rightDepth++;
                currentRight  = currentRight.ComposedParentElement;
            }

            currentLeft  = left;
            currentRight = right;

            while (leftDepth > rightDepth)
            {
                currentLeft = currentLeft.ComposedParentElement!;
                leftDepth--;

                leftToAncestor.Add(currentLeft);
            }

            while (leftDepth < rightDepth)
            {
                currentRight = currentRight.ComposedParentElement!;
                rightDepth--;

                rightToAncestor.Add(currentRight);
            }

            while (currentLeft != currentRight)
            {
                currentLeft  = currentLeft.ComposedParentElement;
                currentRight = currentRight.ComposedParentElement;

                if (currentLeft == null || currentRight == null)
                {
                    leftToAncestor.Clear();
                    rightToAncestor.Clear();

                    throw new InvalidOperationException(ERROR_MESSAGE);
                }

                leftToAncestor.Add(currentLeft);
                rightToAncestor.Add(currentRight);
            }
        }
    }

    internal ComposedTreeEnumerator GetComposedTreeEnumerator() =>
        new(this);

    internal ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator(Stack<(Slot, int)>? stack = null, Action<Element>? parentCallback = null) =>
        new(this, stack, parentCallback);

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
