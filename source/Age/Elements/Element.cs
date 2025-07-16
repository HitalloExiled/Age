using Age.Numerics;
using Age.Scene;
using Age.Styling;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Runtime.CompilerServices;

namespace Age.Elements;

public abstract partial class Element : Layoutable, IComparable<Element>, IEnumerable<Element>
{
    protected bool IsFocusable { get; set; }

    internal protected ShadowTree? ShadowTree { get; set; }

    public Canvas? Canvas    { get; private set; }
    public bool    IsFocused { get; private set; }
    public bool    IsHovered { get; private set; }

    public Point<uint> Scroll
    {
        get => this.ContentOffset;
        set => this.ContentOffset = value;
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

            this.RequestUpdate(true);
        }
    }

    protected Element() => this.NodeFlags = NodeFlags.IgnoreUpdates;

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

    [MemberNotNull(nameof(ShadowTree))]
    protected void AttachShadowTree(bool? inheritsHostStyle = null) => this.ShadowTree = new(this, inheritsHostStyle == true);

    internal ComposedTreeEnumerator GetComposedTreeEnumerator() =>
        new(this);

    internal ComposedTreeTraversalEnumerator GetComposedTreeTraversalEnumerator(Stack<(Slot, int)>? stack = null) =>
        new(this, stack);

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
        this.RemoveState(ElementState.Focus);
        this.BluredEvent?.Invoke(new() { Target = this });
    }

    public void Click()
    {
        this.AddState(ElementState.Active);
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
        this.AddState(ElementState.Focus);
        this.IsFocused = true;
        this.FocusedEvent?.Invoke(new() { Target = this });
    }

    public BoxModel GetBoxModel()
    {
        var boundings = this.GetUpdatedBoundings();

        var padding = this.Padding;
        var border  = this.Border;
        var content = this.Content;
        var margin  = this.Margin;

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
        if (!this.CanScrollX || !this.CanScrollY)
        {
            return;
        }

        var boxModel = this.GetBoxModel();

        var boundsLeft   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
        var boundsRight  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
        var boundsTop    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
        var boundsBottom = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

        var scroll = this.Scroll;

        if (this.CanScrollX)
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

        if (this.CanScrollY)
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
        this.ScrollTo(element.GetUpdatedBoundings());
}
