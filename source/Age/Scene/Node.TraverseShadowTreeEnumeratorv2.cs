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
        private readonly List<StackEntry> stack = [];
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
        private readonly bool IsCurrentSlot(Slot slot) =>
            this.stack.Count > 0 && this.stack[^1].Slot == slot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsInCurrentSlot(Node node) =>
            this.stack.Count > 0 && node is Layoutable layoutable && this.stack[^1].Slot == layoutable.AssignedSlot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetFirstChild(Node node, bool ignoreShadowTree)
        {
            if (node is Slot slot)
            {
                if (slot.Nodes.Count > 0)
                {
                    this.stack.Add((slot, 0));

                    return slot.Nodes[0];
                }
            }
            else if (!ignoreShadowTree && node is Element element && element.ShadowTree != null)
            {
                return element.ShadowTree.FirstChild;
            }

            return this.GetNodeOrNext(node.FirstChild);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetNextChild(Node node, out Node? parent)
        {
            parent = node.Parent;

            if (node is ShadowTree shadowTree)
            {
                parent = shadowTree.Host;

                return this.GetFirstChild(shadowTree.Host, true);
            }

            if (this.IsInCurrentSlot(node))
            {
                var (slot, index) = this.stack[^1];

                index++;

                if (index < slot.Nodes.Count)
                {
                    this.stack[^1] = (slot, index);

                    return slot.Nodes[index];
                }

                this.stack.RemoveAt(this.stack.Count - 1);

                parent = slot;

                return null;
            }

            return this.GetNodeOrNext(node.NextSibling);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly Node? GetNodeOrNext(Node? node)
        {
            while (node is Layoutable layoutable && layoutable.AssignedSlot != null && !this.IsCurrentSlot(layoutable.AssignedSlot))
            {
                node = node.NextSibling;
            }

            return node;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (!this.skipToNextSibling && this.GetFirstChild(this.current!, false) is Node first)
            {
                this.current = first;
            }
            else
            {
                this.skipToNextSibling = false;

                while (this.current != null)
                {
                    if (this.GetNextChild(this.current, out var parent) is Node next)
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
