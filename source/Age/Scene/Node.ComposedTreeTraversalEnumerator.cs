using Age.Elements;
using System.Collections;
using System.Runtime.CompilerServices;
using StackEntry = (Age.Elements.Slot Slot, int Index);

namespace Age.Scene;

public abstract partial class Node
{
    public struct ComposedTreeTraversalEnumerator : IEnumerator<Node>, IEnumerable<Node>
    {
        #region 8-bytes
        private readonly Node root;
        private readonly Stack<StackEntry> stack = [];
        private Node? current;
        #endregion

        #region 1-byte
        private bool skipToNextSibling;
        #endregion

        public ComposedTreeTraversalEnumerator(Node root)
        {
            this.root = root;

            this.Reset();
        }

        public readonly Node Current => this.current!;

        readonly object IEnumerator.Current => this.Current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsAssignedToCurrentSlot(Node node) =>
            this.stack.Count > 0 && node is Layoutable layoutable && this.stack.Peek().Slot == layoutable.AssignedSlot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsCurrentSlot(Slot slot) =>
            this.stack.Count > 0 && this.stack.Peek().Slot == slot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetFirstChild(Node node)
        {
            if (node is Slot slot)
            {
                if (slot.Nodes.Count > 0)
                {
                    this.stack.Push((slot, 0));

                    return slot.Nodes[0];
                }
                else
                {
                    return slot.FirstChild;
                }
            }
            else if (node is Element element && element.ShadowTree != null)
            {
                return element.ShadowTree.FirstChild;
            }

            return this.GetNodeOrSkip(node.FirstChild);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetNextSibling(Node node, out Node? parent)
        {
            if (this.IsAssignedToCurrentSlot(node))
            {
                var (slot, index) = this.stack.Pop();

                parent = slot;

                if (++index < slot.Nodes.Count)
                {
                    this.stack.Push((slot, index));

                    return slot.Nodes[index];
                }

                return null;
            }

            parent = node.Parent;

            if (this.GetNodeOrSkip(node.NextSibling) is Node nextSibling)
            {
                return nextSibling;
            }

            if (parent is ShadowTree shadowTree)
            {
                parent = shadowTree.Host;

                return this.GetNodeOrSkip(shadowTree.Host.FirstChild);
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetNodeOrSkip(Node? node)
        {
            while (node is Layoutable layoutable && layoutable.AssignedSlot != null && !this.IsCurrentSlot(layoutable.AssignedSlot))
            {
                node = node.NextSibling;
            }

            return node;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        readonly IEnumerator<Node> IEnumerable<Node>.GetEnumerator() =>
            this.GetEnumerator();

        public readonly void Dispose()
        { }

        public readonly ComposedTreeTraversalEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (!this.skipToNextSibling && this.GetFirstChild(this.current!) is Node firstChild)
            {
                this.current = firstChild;

                return true;
            }

            this.skipToNextSibling = false;

            while (this.current != null)
            {
                if (this.GetNextSibling(this.current, out var parent) is Node nextSibling)
                {
                    this.current = nextSibling;

                    return true;
                }

                if (parent == this.root)
                {
                    return false;
                }

                this.current = parent;
            }

            return false;
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
