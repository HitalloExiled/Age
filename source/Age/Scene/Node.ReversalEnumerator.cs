using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
    public struct ReversalEnumerator(Node node) : IEnumerator<Node>, IEnumerable<Node>
    {
        private Node? current;
        private bool  last = true;

        public readonly Node Current => this.current!;
        public readonly Node Node    => node;

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        readonly IEnumerator IEnumerable.GetEnumerator() => this;

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (this.last)
            {
                this.current = node.LastChild;
                this.last    = false;
            }
            else
            {
                this.current = this.current?.PreviousSibling;
            }

            return this.current != null;
        }

        public void Reset()
        {
            this.last    = true;
            this.current = null;
        }
    }
}
