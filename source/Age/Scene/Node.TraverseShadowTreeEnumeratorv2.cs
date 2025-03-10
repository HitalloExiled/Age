using Age.Elements;
using System.Collections;
using System.Runtime.CompilerServices;
using StackEntry = (Age.Elements.Slot Slot, int Index);

namespace Age.Scene;

public abstract partial class Node
{
    public struct TraverseShadowTreeEnumeratorV2 : IEnumerator<Node>, IEnumerable<Node>
    {
        #region 8-bytes
        private readonly Node root;
        private readonly Stack<StackEntry> stack = [];
        private Node? current;
        #endregion

        #region 1-byte
        private bool skipToNextSibling;
        #endregion

        public TraverseShadowTreeEnumeratorV2(Node root)
        {
            this.root = root;

            this.Reset();
        }

        public readonly Node Current => this.current!;

        readonly object IEnumerator.Current => this.Current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetNodeOrSkip(Node? node)
        {
            while (node is Layoutable layoutable && layoutable.AssignedSlot != null && !this.IsCurrentSlot(layoutable.AssignedSlot))
            {
                node = node.NextSibling;
            }

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsCurrentSlot(Slot slot) =>
            this.stack.Count > 0 && this.stack.Peek().Slot == slot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsSlotted(Node node) =>
            this.stack.Count > 0 && node is Layoutable layoutable && this.stack.Peek().Slot == layoutable.AssignedSlot;

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            Node? first = null;

            if (!this.skipToNextSibling)
            {
                if (this.current is Slot slot)
                {
                    if (slot.Nodes.Count > 0)
                    {
                        this.stack.Push((slot, 0));

                        first = slot.Nodes[0];
                    }
                    else
                    {
                        first = slot.FirstChild;
                    }
                }
                else
                {
                    first = this.current is Element element && element.ShadowTree != null
                        ? element.ShadowTree.FirstChild
                        : this.GetNodeOrSkip(this.current!.FirstChild);
                }
            }

            if (first != null)
            {
                this.current = first;
            }
            else
            {
                this.skipToNextSibling = false;

                while (this.current != null)
                {
                    var parent = this.current.Parent;
                    Node? next = null;

                    if (this.current is ShadowTree shadowTree)
                    {
                        parent = shadowTree.Host;

                        next = this.GetNodeOrSkip(shadowTree.Host.FirstChild);
                    }
                    else if (this.IsSlotted(this.current))
                    {
                        var (slot, index) = this.stack.Pop();

                        if (++index < slot.Nodes.Count)
                        {
                            this.stack.Push((slot, index));

                            next = slot.Nodes[index];
                        }
                        else
                        {
                            parent = slot;
                        }
                    }
                    else
                    {
                        next = this.GetNodeOrSkip(this.current.NextSibling);
                    }

                    if (next != null)
                    {
                        this.current = next;

                        break;
                    }

                    this.current = parent == this.root ? null : parent;
                }
            }

            return this.current != null;
        }

        public void Reset()
        {
            this.skipToNextSibling = false;
            this.current           = this.root;

            this.stack.Clear();
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;
    }
}
