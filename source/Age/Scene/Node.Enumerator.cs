using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
    public struct Enumerator(Node node) : IEnumerator<Node>
    {
        private Node? current;
        private bool  first = true;

        public readonly Node Current => this.current ?? throw new InvalidOperationException();
        public readonly Node Node    => node;

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public bool MoveNext()
        {
            if (this.first)
            {
                this.current = node.FirstChild;
                this.first   = false;
            }
            else
            {
                this.current = this.current?.NextSibling;
            }

            return this.current != null;
        }

        public void Reset()
        {
            this.first   = true;
            this.current = null;
        }
    }
}
