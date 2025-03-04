using Age.Elements;
using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
    public struct TraverseShadowTreeEnumerator : IEnumerator<Node>, IEnumerable<Node>
    {
        #region 8-bytes
        private readonly Node root;
        private readonly Stack<(Node Node, bool IsSlotted)> stack;
        private Node? current;
        #endregion

        public TraverseShadowTreeEnumerator(Node root, Stack<(Node Node, bool IsSlotted)>? stack = null)
        {
            this.root  = root;
            this.stack = stack ?? [];
            this.Reset();
        }

        public readonly Node Current => this.current!;

        readonly object IEnumerator.Current => this.Current;

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        private readonly void PushChildren(Node parent)
        {
            var node = parent.LastChild;

            while (node != null)
            {
                this.stack.Push((node, false));

                node = node.PreviousSibling;
            }
        }

        private readonly void PushSlots(Slot slot)
        {
            for (var i = slot.Nodes.Count - 1; i >= 0; i--)
            {
                this.stack.Push((slot.Nodes[i], true));
            }
        }

        public bool MoveNext()
        {
            while (this.stack.Count > 0)
            {
                var (currentNode, isSlotted) = this.stack.Pop();

                if (currentNode is Element element)
                {
                    if (!isSlotted && element.AssignedSlot != null)
                    {
                        continue;
                    }

                    if (element is Slot slot && slot.Nodes.Count > 0)
                    {
                        this.PushSlots(slot);

                        this.current = slot;

                        return true;
                    }

                    if (element.ShadowTree != null)
                    {
                        this.PushChildren(element.ShadowTree);
                    }
                }

                this.PushChildren(currentNode);

                this.current = currentNode;

                return true;
            }

            return false;
        }

        public void Reset()
        {
            this.current = null;
            this.stack.Clear();

            if (this.root is Element rootElement && rootElement.ShadowTree != null)
            {
                this.PushChildren(rootElement);
                this.PushChildren(rootElement.ShadowTree);
            }
            else
            {
                this.PushChildren(this.root);
            }
        }

        public readonly void SkipToNextSibling()
        {
            if (this.current is Slot slot && slot.Nodes.Count > 0)
            {
                while (this.stack.Count > 0 && this.stack.Peek().Node is Element element && element.AssignedSlot == slot)
                {
                    this.stack.Pop();
                }
            }
            else
            {
                while (this.stack.Count > 0 && this.stack.Peek().Node.Parent == this.current)
                {
                    this.stack.Pop();
                }
            }
        }
    }
}
